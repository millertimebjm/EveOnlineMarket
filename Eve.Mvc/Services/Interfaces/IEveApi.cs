
using Eve.Mvc.Models;

namespace Eve.Mvc.Services.Interfaces;

public interface IEveApi
{
    Task<List<EveMarketOrder>> GetMarketOrders(long userId, string accessToken);
    Task<EveUniverseType> GetUniverseType(int typeId, string accessToken);
    Task<int> GetCharacterId(string accessToken);
}