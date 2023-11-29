using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// if (builder.Environment.EnvironmentName != "Development")
// {
//     var connection = new SqliteConnection("DataSource=votos.db");
//     connection.Open();
//     builder.Services.AddDbContext<VotoDb>(opt => opt.UseSqlite(connection));
// }
// else
// {
//     builder.Services.AddDbContext<VotoDb>(opt => opt.UseInMemoryDatabase("Votos"));
// }

// builder.Services.AddDatabaseDeveloperPageExceptionFilter();

List<Voto> _listaDeVotos = new List<Voto>();

var votos = app.MapGroup("/votos");
int contador = 0;

votos.MapGet("/", () => obtenerVotos(_listaDeVotos));
votos.MapPost("/", (Voto nuevoVoto) => crearVoto(nuevoVoto, _listaDeVotos, ref contador));

static IResult obtenerVotos(List<Voto> lista)
{
    return TypedResults.Ok(lista);
}
static IResult crearVoto([FromBody] Voto nuevoVoto, List<Voto> lista, ref int contador)
{
    contador++;
    nuevoVoto.Id = contador;
    lista.Add(nuevoVoto);

    return TypedResults.Ok(nuevoVoto);
}


app.Run();

public class Voto
{
    public int Id {get; set;}
    public required int IdVotante {get; set;}
    public int? IdCandidato {get; set;}
}

public class Votante
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("fechaNacimiento")] public required string FechaNacimiento { get; set; }
}

public class VotoDb : DbContext
{
    public VotoDb(DbContextOptions<VotoDb> options) : base(options) { 
        Database.EnsureCreated();
    }

    public DbSet<Voto> Votos => Set<Voto>();
}
