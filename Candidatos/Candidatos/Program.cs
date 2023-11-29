using Microsoft.AspNetCore.Mvc;
        
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

List<Candidato> _listaDeCandidatos = new List<Candidato>();
_listaDeCandidatos.Add(new Candidato { Id = 1, Nombre = "Nahuel", Apellido = "Ferrero", Partido = "X" });

var candidatos = app.MapGroup("/candidatos");
int contador = 1;

candidatos.MapGet("/", () => obtenerCandidatos(_listaDeCandidatos));
candidatos.MapGet("/{id}", (int id) => obtenerCandidato(id, _listaDeCandidatos));
candidatos.MapPost("/", (Candidato candidato) => crearCandidato(candidato, _listaDeCandidatos, ref contador));
candidatos.MapDelete("/{id}", (int id) => eliminarCandidato(id, _listaDeCandidatos));
candidatos.MapPut("/{id}", (int id, Candidato candidato) => modificarCandidato(id, _listaDeCandidatos, candidato));

static IResult obtenerCandidatos(List<Candidato> lista)
{
    return TypedResults.Ok(lista);
}
static IResult obtenerCandidato([FromRoute] int id, List<Candidato> lista)
{
    Candidato? candidato = lista.Find(c => c.Id == id);
    if(candidato == null) return TypedResults.NotFound("Candidato no encontrado");
    else return TypedResults.Ok(candidato); 
}
static IResult crearCandidato([FromBody] Candidato candidato, List<Candidato> lista, ref int contador)
{
    contador++;
    candidato.Id = contador;
    lista.Add(candidato);
    return TypedResults.Created("/candidatos/{candidato.id}",candidato);
}
static IResult eliminarCandidato([FromRoute] int id, List<Candidato> lista)
{
    Candidato? candidato = lista.Find(c => c.Id == id);
    if(candidato == null) return TypedResults.NotFound("Candidato no encontrado");
    else
    {
        lista.Remove(candidato);
        return TypedResults.Ok("Candidato Eliminado");
    }
}
static IResult modificarCandidato([FromRoute] int id, List<Candidato> lista, [FromBody]Candidato candidato)
{
    Candidato? candidatoBuscado = lista.Find(c => c.Id == id);
    if(candidatoBuscado == null) return TypedResults.NotFound("Candidato no encontrado");
    else
    {
        int indiceCandidato = lista.IndexOf(candidatoBuscado);
        if(candidato.Nombre != null) lista[indiceCandidato].Nombre = candidato.Nombre;
        if(candidato.Apellido != null) lista[indiceCandidato].Apellido = candidato.Apellido;
        if(candidato.Partido != null) lista[indiceCandidato].Partido = candidato.Partido;
        return TypedResults.Ok(candidatoBuscado);
    }
}
    
app.Run();  


class Candidato
{
    public int Id {get; set;}
    public string? Nombre {get; set;}
    public string? Apellido {get; set;}
    public string? Partido {get; set;}
}

