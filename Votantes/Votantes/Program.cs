using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
        
var builder = WebApplication.CreateBuilder(args);

switch (builder.Environment.EnvironmentName)
{
    case "Development":
        builder.Services.AddDbContext<VotantesDb>(opt => opt.UseInMemoryDatabase("Votantes"));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();
        break;
    case "Homologation":
        var connection = new SqliteConnection("DataSource=votantes.db");
        connection.Open();  
        builder.Services.AddDbContext<VotantesDb>(opt => opt.UseSqlite(connection));
        break;
    case "Production":
    default:
        throw new NotImplementedException();
}

var app = builder.Build();
var votantes = app.MapGroup("/votantes");


votantes.MapGet("/", obtenerVotantes);
votantes.MapGet("/{id}", obtenerVotante);
votantes.MapPost("/", crearVotante);
votantes.MapDelete("/{id}",eliminarVotante);
votantes.MapPut("/{id}",modificarVotante);


static async Task<IResult> obtenerVotantes(VotantesDb db)
{
    return TypedResults.Ok(await db.Votantes.ToListAsync());    
}
static async Task<IResult> obtenerVotante([FromRoute] int id, VotantesDb db)
{
    Votante? votante = await db.Votantes.FindAsync(id);
    if(votante == null) return TypedResults.NotFound("Votante no encontrado");
    else return TypedResults.Ok(votante); 
}
static async Task<IResult> crearVotante([FromBody] Votante votante, VotantesDb db)
{
    db.Votantes.Add(votante);
    await db.SaveChangesAsync();
    return TypedResults.Created($"/votantes/{votante.Id}",votante);
}
static async Task<IResult> eliminarVotante([FromRoute] int id, VotantesDb db)
{
    Votante? votante = await db.Votantes.FindAsync(id);
    if(votante == null) return TypedResults.NotFound("Votante no encontrado");
    else
    {
        db.Votantes.Remove(votante);
        await db.SaveChangesAsync();
        return TypedResults.Ok("Votante Eliminado");
    }
}
static async Task<IResult> modificarVotante([FromRoute] int id, VotantesDb db, [FromBody] Votante votante)
{
    Votante? votanteBuscado = await db.Votantes.FindAsync(id);
    if(votanteBuscado == null) return TypedResults.NotFound("Votante no encontrado");
    
    if(votante.Nombre != null) votanteBuscado.Nombre = votante.Nombre;;
    if(votante.Apellido != null) votanteBuscado.Apellido = votante.Apellido;
    if(votante.Dni != null) votanteBuscado.Dni = votante.Dni;
    if(votante.FechaNacimiento != null) votanteBuscado.FechaNacimiento = votante.FechaNacimiento;
    return TypedResults.Ok(votanteBuscado);
}
    
app.Run();  


public class Votante
{
    public int Id {get; set;}
    public string? Nombre {get; set;}
    public string? Apellido {get; set;}
    public string? FechaNacimiento {get; set;}
    public int? Dni {get; set;}
}

public class VotantesDb : DbContext
{
    public VotantesDb(DbContextOptions<VotantesDb> options) : base(options) 
    {
        Database.EnsureCreated();
    }
    public DbSet<Votante> Votantes => Set<Votante>();
}

