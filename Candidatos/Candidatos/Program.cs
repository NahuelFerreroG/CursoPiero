using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
        
var builder = WebApplication.CreateBuilder(args);

switch (builder.Environment.EnvironmentName)
{
    case "Development":
        builder.Services.AddDbContext<CandidatosDb>(opt => opt.UseInMemoryDatabase("Candidatos"));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();
        break;
    case "Homologation":
        var connection = new SqliteConnection("DataSource=candidatos.db");
        connection.Open();  
        builder.Services.AddDbContext<CandidatosDb>(opt => opt.UseSqlite(connection));
        break;
    case "Production":
    default:
        throw new NotImplementedException();
}

var app = builder.Build();

var candidatos = app.MapGroup("/candidatos");

candidatos.MapGet("/",obtenerCandidatos);
candidatos.MapGet("/{id}",obtenerCandidato);
candidatos.MapPost("/", crearCandidato);
candidatos.MapDelete("/{id}",eliminarCandidato);
candidatos.MapPut("/{id}", modificarCandidato);

static async Task<IResult> obtenerCandidatos(CandidatosDb db)
{
    return TypedResults.Ok(await db.Candidatos.ToListAsync());
}
static async Task<IResult> obtenerCandidato([FromRoute] int id, CandidatosDb db)
{
    Candidato? candidato = await db.Candidatos.FindAsync(id);
    if(candidato == null) return TypedResults.NotFound("Candidato no encontrado");
    else return TypedResults.Ok(candidato); 
}
static async Task<IResult> crearCandidato([FromBody] Candidato candidato, CandidatosDb db)
{
    db.Candidatos.Add(candidato);
    await db.SaveChangesAsync();
    return TypedResults.Created($"/candidatos/{candidato.Id}",candidato);
}
static async Task<IResult> eliminarCandidato([FromRoute] int id, CandidatosDb db)
{
    Candidato? candidato = await db.Candidatos.FindAsync(id);
    if(candidato == null) return TypedResults.NotFound("Candidato no encontrado");
    else
    {
        db.Candidatos.Remove(candidato);
        await db.SaveChangesAsync();
        return TypedResults.Ok("Candidato Eliminado");
    }
}
static async Task<IResult> modificarCandidato([FromRoute] int id, CandidatosDb db, [FromBody]Candidato candidato)
{
    Candidato? candidatoBuscado = await db.Candidatos.FindAsync(id);
    if(candidatoBuscado == null) return TypedResults.NotFound("Candidato no encontrado");

    if(candidato.Nombre != null) candidatoBuscado.Nombre = candidato.Nombre;
    if(candidato.Apellido != null) candidatoBuscado.Apellido = candidato.Apellido;
    if(candidato.Partido != null) candidatoBuscado.Partido = candidato.Partido;
    return TypedResults.Ok(candidatoBuscado);
}
    
app.Run();  


public class Candidato
{
    public int Id {get; set;}
    public string? Nombre {get; set;}
    public string? Apellido {get; set;}
    public string? Partido {get; set;}
}

public class CandidatosDb : DbContext
{
    public CandidatosDb(DbContextOptions<CandidatosDb> options) : base(options) 
    {
        Database.EnsureCreated();
    }
    public DbSet<Candidato> Candidatos => Set<Candidato>();
}
