using System.Net.Sockets;
using Eve.Models.EveApi;

namespace Eve.Mvc.Models;

public class PlanetaryInteractionsViewModel 
{
    public Task<List<PlanetaryInteraction>> PlanetaryInteractionsTask { get; set; } 
        = Task.FromResult(new List<PlanetaryInteraction>());
    public Task<List<Planet>> PlanetsTask { get; set; }
        = Task.FromResult(new List<Planet>());
    public Task<List<Type>> TypesTask { get; set; }
        = Task.FromResult(new List<Type>());
}