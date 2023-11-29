using Microsoft.AspNetCore.Mvc;
        
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

List<Votante> _listaDeVotantes = new List<Votante>();
_listaDeVotantes.Add(new Votante { Id = 1, Nombre = "Nahuel", Apellido = "Ferrero", Dni = 32591308 });

var votantes = app.MapGroup("/votantes");
int contador = 1;

votantes.MapGet("/", () => obtenerVotantes(_listaDeVotantes));
votantes.MapGet("/{id}", (int id) => obtenerVotante(id, _listaDeVotantes));
votantes.MapPost("/", (Votante votante) => crearVotante(votante, _listaDeVotantes, ref contador));
votantes.MapDelete("/{id}", (int id) => eliminarVotante(id, _listaDeVotantes));
votantes.MapPut("/{id}", (int id, Votante votante) => modificarVotante(id, _listaDeVotantes, votante));

static IResult obtenerVotantes(List<Votante> lista)
{
    return TypedResults.Ok(lista);
}
static IResult obtenerVotante([FromRoute] int id, List<Votante> lista)
{
    Votante? votante = lista.Find(c => c.Id == id);
    if(votante == null) return TypedResults.NotFound("Votante no encontrado");
    else return TypedResults.Ok(votante); 
}
static IResult crearVotante([FromBody] Votante votante, List<Votante> lista, ref int contador)
{
    contador++;
    votante.Id = contador;
    lista.Add(votante);
    return TypedResults.Created("/votantes/{votante.id}",votante);
}
static IResult eliminarVotante([FromRoute] int id, List<Votante> lista)
{
    Votante? votante = lista.Find(c => c.Id == id);
    if(votante == null) return TypedResults.NotFound("Votante no encontrado");
    else
    {
        lista.Remove(votante);
        return TypedResults.Ok("Votante Eliminado");
    }
}
static IResult modificarVotante([FromRoute] int id, List<Votante> lista, [FromBody] Votante votante)
{
    Votante? votanteBuscado = lista.Find(c => c.Id == id);
    if(votanteBuscado == null) return TypedResults.NotFound("Votante no encontrado");
    else
    {
        int indiceVotante = lista.IndexOf(votanteBuscado);
        if(votante.Nombre != null) lista[indiceVotante].Nombre = votante.Nombre;
        if(votante.Apellido != null) lista[indiceVotante].Apellido = votante.Apellido;
        if(votante.Dni != null) lista[indiceVotante].Dni = votante.Dni;
        return TypedResults.Ok(votanteBuscado);
    }
}
    
app.Run();  


class Votante
{
    public int Id {get; set;}
    public string? Nombre {get; set;}
    public string? Apellido {get; set;}
    public int? Dni {get; set;}
}
