
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();

        List<Votante> _listaDeVotantes = new List<Votante>();
        //_listaDeVotantes.Add(new Votante { Id = 1, Nombre = "Nahuel", Apellido = "Ferrero", Dni = 32591308 });

        var votantes = app.MapGroup("/votantes");
        List<int> contador = new List<int>();

        votantes.MapGet("/", () => TypedResults.Ok(_listaDeVotantes));
        votantes.MapGet("/{id}",
            (int id) => 
            {
                Votante? votante = _listaDeVotantes.Find(votante => votante.Id == id);
                return TypedResults.Accepted($"/votantes/{votante.Id}", votante);
            }
        );
        votantes.MapPost("/",
            (Votante nuevoVotante) =>
            {
                contador.Add(1);
                nuevoVotante.Id = contador.Count;
                _listaDeVotantes.Add(nuevoVotante);
                return TypedResults.Created($"/votantes/{nuevoVotante.Id}", nuevoVotante);
            }
        );
        votantes.MapDelete("/{id}",
            (int id) =>
            {
                Votante? votante = _listaDeVotantes.Find(votante => votante.Id == id);
                if (votante != null) _listaDeVotantes.Remove(votante);
                return TypedResults.Ok();
            });
        votantes.MapPut("/{id}",
            (int id, Votante votanteEditado) =>
            {
                foreach (var votante in _listaDeVotantes)
                {
                    if (votante.Id == id)
                    {
                        if (votanteEditado.Nombre != null) votante.Nombre = votanteEditado.Nombre;
                        if (votanteEditado.Apellido != null) votante.Apellido = votanteEditado.Apellido;
                        if (votanteEditado.Dni != null) votante.Dni = votanteEditado.Dni;
                    }
                }
                return TypedResults.Created($"/votantes/{votanteEditado.Id}", votanteEditado);
            }
        );

        app.Run();  


class Votante
{
    public int Id {get; set;}
    public string? Nombre {get; set;}
    public string? Apellido {get; set;}
    public int? Dni {get; set;}

}

