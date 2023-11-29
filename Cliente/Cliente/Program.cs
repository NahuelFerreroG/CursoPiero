using System.Text.Json;
using HttpClient client = new();

var candidatos = await obtenerCandidatos(client);



static async Task<List<Candidato>> obtenerCandidatos(HttpClient client)
{
    await using Stream stream = await client.GetStreamAsync("http://localhost:5000/candidatos");
    
    var candidatos = await JsonSerializer.DeserializeAsync<List<Candidato>>(stream);

    return candidatos?? new();
}
