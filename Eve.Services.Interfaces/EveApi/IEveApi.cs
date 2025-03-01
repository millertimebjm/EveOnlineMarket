using Eve.Models.EveApi;
using Eve.Services.Interfaces.Wrappers;

namespace Eve.Services.Interfaces.EveApi;

public interface IEveApi
{
    Task<int> GetCharacterId(string accessToken);
    Task<List<PlanetaryInteraction>> GetPlanetaryInteractions(long userId, string accessToken);
    Task<Planet> GetPlanet(int planetId, string accessToken);
    Task<List<Planet>> GetPlanets(List<int> planetIds, string accessToken);
}