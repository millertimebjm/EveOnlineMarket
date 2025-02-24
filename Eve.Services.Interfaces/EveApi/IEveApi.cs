using Eve.Models.EveApi;

namespace Eve.Services.Interfaces.EveApi;

public interface IEveApi
{
    Task<List<Order>> GetMarketOrders(long userId, string accessToken);
    Task<List<long>> GetMarketOrderIds(long userId, string accessToken);

    Task<EveType> GetEveType(int typeId, string accessToken);
    Task<int> GetCharacterId(string accessToken);

    Task<List<Order>> GetBuySellOrders(int typeId, string accessToken);
    Task<EveType> GetEveType(int typeId, string accessToken, HttpClient httpClient);
    IAsyncEnumerable<int> GetEveTypeIds(string accessToken);
    Task<List<PlanetaryInteraction>> GetPlanetaryInteractions(long userId, string accessToken);
    Task<Planet> GetPlanet(int planetId, string accessToken);
    Task<List<Planet>> GetPlanets(List<int> planetIds, string accessToken);

}