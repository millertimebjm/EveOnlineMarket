using Eve.Models.EveApi;

namespace Eve.Services.Interfaces.Orders;

public interface IOrdersService
{
    Task<List<Order>> GetMarketOrders(long userId, string accessToken);
    Task<List<long>> GetMarketOrderIds(long userId, string accessToken);
    Task<List<Order>> GetBuySellOrders(int typeId, string accessToken);
    Task<IDictionary<long, int>> GetMarketOrderRanks(IDictionary<int, long> typeOrderIds, string accessToken);
}