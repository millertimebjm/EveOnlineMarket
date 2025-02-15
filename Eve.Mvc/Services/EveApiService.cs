
using System.Collections.Concurrent;
using System.Text.Json;
using Eve.Mvc.Models;
using Eve.Mvc.Services.Interfaces;

namespace Eve.Mvc.Services;

public class EveApiService : IEveApi
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ConcurrentDictionary<int, EveUniverseType> _typeCache;

    public EveApiService(
        IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _typeCache = new();
    }

    public async Task<List<EveMarketOrder>> GetMarketOrders(long userId, string accessToken)
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync(new Uri($"https://esi.evetech.net/latest/characters/{userId}/orders/?datasource=tranquility&token={accessToken}"));
        response.EnsureSuccessStatusCode();
        var eveMarketOrders = JsonSerializer.Deserialize<List<EveMarketOrder>>(
            await response.Content.ReadAsStringAsync(),
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        return eveMarketOrders;
    }

    public async Task<List<long>> GetMarketOrderIds(long userId, string accessToken)
    {
        var marketOrders = await GetMarketOrders(userId, accessToken);
        return marketOrders.Select(x => x.OrderId).ToList();
    }

    public async Task<EveUniverseType> GetUniverseType(
        int typeId,
        string accessToken)
    {
        if (_typeCache.TryGetValue(typeId, out var value)) return value;

        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync(new Uri($"https://esi.evetech.net/latest/universe/types/{typeId}/?datasource=tranquility&token={accessToken}"));
        response.EnsureSuccessStatusCode();
        var type = JsonSerializer.Deserialize<EveUniverseType>(
            await response.Content.ReadAsStringAsync(),
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        return type;
    }

    public async Task<int> GetCharacterId(string accessToken)
    {
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
        var response = await client.GetAsync(new Uri($"https://login.eveonline.com/oauth/verify"));
        response.EnsureSuccessStatusCode();
        var responseDynamic = await response.Content.ReadFromJsonAsync<EveCharacterResponse>();
        return responseDynamic.CharacterId;
    }

    public async Task<List<EveMarketOrder>> GetBuySellOrders(int typeId, string accessToken)
    {
        int regionId = 10000002;
        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync(new Uri($"https://esi.evetech.net/latest/markets/{regionId}/orders?datasource=tranquility&token={accessToken}&type_id={typeId}"));
        response.EnsureSuccessStatusCode();
        var buySellOrders = JsonSerializer.Deserialize<List<EveMarketOrder>>(
            await response.Content.ReadAsStringAsync(),
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        return buySellOrders;

        //         base_url = "https://api.evemarketer.com/ec/marketstat"
        // params = {
        //     'typeid': YOUR_TYPE_ID,
        //     'regionlimit': 10000002,  # The Forge region ID
        //     'usesystem': 30000142     # Jita system ID
        // }
    }
}