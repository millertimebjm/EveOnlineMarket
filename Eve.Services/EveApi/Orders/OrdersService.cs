using System.Collections.Concurrent;
using System.Text.Json;
using Eve.Models.EveApi;
using Eve.Services.Interfaces.Orders;
using Eve.Services.Interfaces.Wrappers;

namespace Eve.Services.Orders;

public class OrdersService : IOrdersService
{
    private readonly IHttpClientWrapper _httpClientWrapper;
    public OrdersService(IHttpClientWrapper httpClientWrapper)
    {
        _httpClientWrapper = httpClientWrapper;
    }

    public async Task<List<Order>> GetMarketOrders(long userId, string accessToken)
    {
        var response = await _httpClientWrapper.GetAsync(new Uri($"https://esi.evetech.net/latest/characters/{userId}/orders/?datasource=tranquility&token={accessToken}"));
        response.EnsureSuccessStatusCode();
        var orders = JsonSerializer.Deserialize<List<Order>>(
            await response.Content.ReadAsStringAsync(),
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        if (orders == null) throw new Exception("eveMarketOrders response from Eve Online API is null");
        return orders;
    }

    public async Task<List<long>> GetMarketOrderIds(long userId, string accessToken)
    {
        var marketOrders = await GetMarketOrders(userId, accessToken);
        return marketOrders.Select(x => x.OrderId).ToList();
    }

    public async Task<List<Order>> GetBuySellOrders(int typeId, string accessToken)
    {
        int regionId = 10000002;
        var response = await _httpClientWrapper.GetAsync(new Uri($"https://esi.evetech.net/latest/markets/{regionId}/orders?datasource=tranquility&token={accessToken}&type_id={typeId}"));
        response.EnsureSuccessStatusCode();
        var buySellOrders = JsonSerializer.Deserialize<List<Order>>(
            await response.Content.ReadAsStringAsync(),
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        if (buySellOrders == null) throw new Exception("buySellOrders response from Eve Online API is null");

        return buySellOrders;

        //         base_url = "https://api.evemarketer.com/ec/marketstat"
        // params = {
        //     'typeid': YOUR_TYPE_ID,
        //     'regionlimit': 10000002,  # The Forge region ID
        //     'usesystem': 30000142     # Jita system ID
        // }
    }

    public async Task<IDictionary<long, int>> GetMarketOrderRanks(
        IDictionary<int, long> typeOrderIds,
        string accessToken)
    {
        IDictionary<long, int> orderRanks = new ConcurrentDictionary<long, int>();
        await Parallel.ForEachAsync(typeOrderIds, async (typeOrderId, token) => {
            var marketOrders = await GetBuySellOrders(typeOrderId.Key, accessToken);
            marketOrders = marketOrders
                .Where(mo => !mo.IsBuyOrder)
                .OrderBy(mo => mo.Price)
                .ToList();
            for (int i = 0; i < marketOrders.Count(); i++)
            {
                if (typeOrderId.Value == marketOrders[i].OrderId)
                {
                    orderRanks.TryAdd(typeOrderId.Value, i+1);
                    break;
                }
            }
        });
        return orderRanks;
    }
}