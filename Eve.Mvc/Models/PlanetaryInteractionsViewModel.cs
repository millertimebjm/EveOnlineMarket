using System.Collections.Concurrent;
using System.Net.Sockets;
using Eve.Models.EveApi;

namespace Eve.Mvc.Models;

public class PlanetaryInteractionsViewModel 
{
    public Task<List<PlanetaryInteraction>> PlanetaryInteractionsTask { get; set; } 
        = Task.FromResult(new List<PlanetaryInteraction>());
    public Task<List<Planet>> PlanetsTask { get; set; }
        = Task.FromResult(new List<Planet>());
    public Task<List<EveType>> TypesTask { get; set; }
        = Task.FromResult(new List<EveType>());
    public Task<List<Schematic>> SchematicsListTask { get; set; }
        = Task.FromResult(new List<Schematic>());
    public Task<IDictionary<int, decimal>> BestBuyOrderValueByTypeTask { get; set; }
        = Task.FromResult((IDictionary<int, decimal>)new Dictionary<int, decimal>());
}