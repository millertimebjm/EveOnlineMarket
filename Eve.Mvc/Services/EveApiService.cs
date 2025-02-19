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
        if (eveMarketOrders == null) throw new Exception("eveMarketOrders response from Eve Online API is null");
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
        return await GetUniverseType(typeId, accessToken, client);
    }

    public async Task<EveUniverseType> GetUniverseType(
        int typeId,
        string accessToken,
        HttpClient client)
    {
        var response = await client.GetAsync(new Uri($"https://esi.evetech.net/latest/universe/types/{typeId}/?datasource=tranquility&token={accessToken}"));
        response.EnsureSuccessStatusCode();
        var type = JsonSerializer.Deserialize<EveUniverseType>(
            await response.Content.ReadAsStringAsync(),
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        if (type == null) throw new Exception("type response from Eve Online API is null");
        return type;
    }

    public async IAsyncEnumerable<int> GetUniverseTypeIds(string accessToken)
    {
        var page = 1;
        var typeIds = new List<int>();
        do
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(new Uri($"https://esi.evetech.net/latest/universe/types/?datasource=tranquility&token={accessToken}&page={page}"));
            if ((int)response.StatusCode == 420)
            {
                await Task.Delay(60 * 1000);
                continue;
            }
            response.EnsureSuccessStatusCode();

            typeIds = JsonSerializer.Deserialize<List<int>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            if (typeIds == null) throw new Exception("planetInteraction response from Eve Online API is null");
            foreach (var typeId in typeIds)
            {
                Console.WriteLine($"found a typeid: {typeId}");
                yield return typeId;
            }
            page++;
            await Task.Delay(5 * 1000);
        } while (typeIds.Any());
    }

    public async Task<int> GetCharacterId(string accessToken)
    {
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
        var response = await client.GetAsync(new Uri($"https://login.eveonline.com/oauth/verify"));
        response.EnsureSuccessStatusCode();
        var character = await response.Content.ReadFromJsonAsync<EveCharacterResponse>();
        if (character == null) throw new Exception("character response from Eve Online API is null");
        return character.CharacterId;
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
        if (buySellOrders == null) throw new Exception("buySellOrders response from Eve Online API is null");

        return buySellOrders;

        //         base_url = "https://api.evemarketer.com/ec/marketstat"
        // params = {
        //     'typeid': YOUR_TYPE_ID,
        //     'regionlimit': 10000002,  # The Forge region ID
        //     'usesystem': 30000142     # Jita system ID
        // }
    }

    public async Task<List<PlanetaryInteractionsModel>> GetPlanetaryInteractions(long characterId, string accessToken)
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync(new Uri($"https://esi.evetech.net/latest/characters/{characterId}/planets/?datasource=tranquility&token={accessToken}"));
        response.EnsureSuccessStatusCode();
        var headers = JsonSerializer.Deserialize<List<PlanetaryInteractionHeaderModel>>(
            await response.Content.ReadAsStringAsync(),
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        if (headers == null) throw new Exception("planetInteraction response from Eve Online API is null");
        var planetInteractions = new List<PlanetaryInteractionsModel>();
        foreach (var header in headers) 
        {
            response = await client.GetAsync(new Uri($"https://esi.evetech.net/latest/characters/{characterId}/planets/{header.planet_id}?datasource=tranquility&token={accessToken}"));
            response.EnsureSuccessStatusCode();
            var planetInteraction = JsonSerializer.Deserialize<PlanetaryInteractionsModel>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            if (planetInteraction == null) throw new Exception("planetInteraction response from Eve Online API is null");
            planetInteraction.Header = header;
            planetInteractions.Add(planetInteraction);
        }
        return planetInteractions;
    }

    public async Task<PlanetModel> GetPlanet(int planetId, string accessToken)
    {
        await Task.Delay(1);
        throw new NotImplementedException();
    }
}