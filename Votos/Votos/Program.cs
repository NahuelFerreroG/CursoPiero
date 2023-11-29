var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();


app.Run();

class Voto
{
    int Id {get; set;}
    Votante Persona {get; set;}
    Candidato Eleccion {get; set;}
}

class Candidato
{
    public int Id {get; set;}
    public string? Nombre {get; set;}
    public string? Apellido {get; set;}
    public string? Partido {get; set;}
}
class Votante
{
    public int Id {get; set;}
    public string? Nombre {get; set;}
    public string? Apellido {get; set;}
    public int? Dni {get; set;}
}