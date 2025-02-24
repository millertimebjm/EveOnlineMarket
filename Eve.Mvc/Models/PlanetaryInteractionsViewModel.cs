using Eve.Models.EveApi;

namespace Eve.Mvc.Models;

public class PlanetaryInteractionsViewModel 
{
    public Task<List<PlanetaryInteraction>>? PlanetaryInteractionsTask { get; set; }
    public Task<List<Planet>>? PlanetsTask { get; set; }
}