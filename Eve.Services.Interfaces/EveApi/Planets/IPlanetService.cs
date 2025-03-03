using Eve.Models.EveApi;

namespace Eve.Services.Interfaces.EveApi.Planets;

public interface IPlanetService
{
    Task<List<PlanetaryInteraction>> GetPlanetaryInteractions(long userId, string accessToken);
    Task<Planet> GetPlanet(int planetId, string accessToken);
    Task<List<Planet>> GetPlanets(List<int> planetIds, string accessToken);
}