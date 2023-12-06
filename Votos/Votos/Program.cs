using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);  

switch (builder.Environment.EnvironmentName)
{
    case "Development":
        builder.Services.AddDbContext<VotoDb>(opt => opt.UseInMemoryDatabase("Votos"));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();
        break;
    case "Homologation":
        var connection = new SqliteConnection("DataSource=votos.db");
        connection.Open();  
        builder.Services.AddDbContext<VotoDb>(opt => opt.UseSqlite(connection));
        break;
    case "Production":
    default:
        throw new NotImplementedException();
}

var app = builder.Build();
var votos = app.MapGroup("/votos");

HttpClient client = new HttpClient();

votos.MapGet("/", obtenerVotos);
votos.MapPost("/", async ([FromBody] Voto nuevoVoto, VotoDb db) => {
    var request = new HttpRequestMessage(HttpMethod.Get, new Uri($"http://localhost:5206/votantes/{nuevoVoto.VotanteId}"));
    request.Headers.Accept.Clear();
    var response = await client.SendAsync(request, CancellationToken.None);
    response.EnsureSuccessStatusCode();
    var votante = JsonSerializer.Deserialize<Votante>(await response.Content.ReadAsStringAsync());
    DateTime FechaNacimiento = DateTime.ParseExact(votante.FechaNacimiento, "yyyy-MM-dd", CultureInfo.InvariantCulture);
    if ((DateTime.Now.Year - FechaNacimiento.Year) > 16)
    {
        db.Votos.Add(nuevoVoto);
        db.SaveChanges();
        return TypedResults.Ok(nuevoVoto.Id);
    }
    else 
    {
        return Results.Ok("Es menor y no puede votar");
    }
});

static async Task<IResult> obtenerVotos(VotoDb db)
{
    return TypedResults.Ok(await db.Votos.ToListAsync());
}

/* static async Task<IResult> crearVoto([FromBody] Voto nuevoVoto, VotoDb db)
{   

        await using Stream stream = await client.GetStreamAsync($"http://localhost:5206/votantes/{nuevoVoto.Id}");
    
        Votante? votante = await JsonSerializer.DeserializeAsync<Votante>(stream);

        if(votante is null) return TypedResults.NotFound("Votante no encontrado o mal cargado");
        DateTime FechaNacimiento = DateTime.ParseExact(votante.FechaNacimiento, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        int edad = DateTime.Now.Year - FechaNacimiento.Year;
        if(edad>= 16)
        {
            db.Votos.Add(nuevoVoto);
            await db.SaveChangesAsync();
            return TypedResults.Created(nuevoVoto.Id.ToString());
        }
        return TypedResults.Conflict("Votante menor de edad");
} */

app.Run();

public class Voto
{
    public int Id { get; set; }
    public int VotanteId { get; set; }
    public int? CandidatoId { get; set; }
}

public class VotoDb : DbContext
{
    public VotoDb(DbContextOptions<VotoDb> options) : base(options) 
    {
        Database.EnsureCreated();
    }
    public DbSet<Voto> Votos => Set<Voto>();
}

public class Votante
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("fechaNacimiento")] public required string FechaNacimiento { get; set; } 

}