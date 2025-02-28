using System.Collections.Concurrent;
using System.Text.Json;
using Eve.Services.Interfaces.EveApi;
using Eve.Models.EveApi;
using System.Net.Http.Json;
using Microsoft.Extensions.Http;
using Eve.Repositories.Interfaces.Planets;
using Eve.Services.Interfaces.Wrappers;
using Eve.Repositories.Interfaces.Types;

namespace Eve.Services.EveApi;

public class EveApiService : IEveApi
{
    private readonly IHttpClientWrapper _httpClientWrapper;
    private readonly ConcurrentDictionary<int, EveType> _typeCache;
    private readonly IPlanetRepository _planetRepository;
    private readonly ITypeRepository _typeRepository;

    public EveApiService(
        IHttpClientWrapper httpClientWrapper,
        IPlanetRepository planetRepository,
        ITypeRepository typeRepository)
    {
        _httpClientWrapper = httpClientWrapper;
        _typeCache = new();
        _planetRepository = planetRepository;
        _typeRepository = typeRepository;
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

    public async Task<EveType> GetEveType(
        int typeId,
        string accessToken)
    {
        if (_typeCache.TryGetValue(typeId, out var value)) return value;

        return await GetEveType(typeId, accessToken, _httpClientWrapper);
    }

    public async Task<EveType> GetEveType(
        int typeId,
        string accessToken,
        IHttpClientWrapper clientWrapper)
    {
        var response = await clientWrapper.GetAsync(new Uri($"https://esi.evetech.net/latest/universe/types/{typeId}/?datasource=tranquility&token={accessToken}"));
        response.EnsureSuccessStatusCode();
        var type = JsonSerializer.Deserialize<EveType>(
            await response.Content.ReadAsStringAsync(),
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        if (type == null) throw new Exception("type response from Eve Online API is null");
        return type;
    }

    public async IAsyncEnumerable<int> GetEveTypeIds(string accessToken)
    {
        var page = 1;
        var typeIds = new List<int>();
        do
        {
            var response = await _httpClientWrapper.GetAsync(new Uri($"https://esi.evetech.net/latest/universe/types/?datasource=tranquility&token={accessToken}&page={page}"));
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
        _httpClientWrapper.AddDefaultRequestHeaders("Authorization", $"Bearer {accessToken}");
        var response = await _httpClientWrapper.GetAsync(new Uri($"https://login.eveonline.com/oauth/verify"));
        response.EnsureSuccessStatusCode();
        var character = await response.Content.ReadFromJsonAsync<Character>();
        if (character == null) throw new Exception("character response from Eve Online API is null");
        return character.CharacterId;
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

    public async Task<List<PlanetaryInteraction>> GetPlanetaryInteractions(long characterId, string accessToken)
    {
        var response = await _httpClientWrapper.GetAsync(new Uri($"https://esi.evetech.net/latest/characters/{characterId}/planets/?datasource=tranquility&token={accessToken}"));
        response.EnsureSuccessStatusCode();
        var headers = JsonSerializer.Deserialize<List<PlanetaryInteractionHeader>>(
            await response.Content.ReadAsStringAsync(),
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        if (headers == null) throw new Exception("planetInteraction response from Eve Online API is null");
        var planetInteractions = new ConcurrentBag<PlanetaryInteraction>();
        
        var options = new ParallelOptions() {
            MaxDegreeOfParallelism = 5,
        };
        await Parallel.ForEachAsync(headers, options, async (header, _) => {
            response = await _httpClientWrapper.GetAsync(new Uri($"https://esi.evetech.net/latest/characters/{characterId}/planets/{header.planet_id}?datasource=tranquility&token={accessToken}"));
            response.EnsureSuccessStatusCode();
            var planetInteraction = JsonSerializer.Deserialize<PlanetaryInteraction>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            if (planetInteraction == null) throw new Exception("planetInteraction response from Eve Online API is null");
            planetInteraction.Header = header;
            planetInteractions.Add(planetInteraction);
        });
        // foreach (var header in headers) 
        // {
        //     response = await _httpClientWrapper.GetAsync(new Uri($"https://esi.evetech.net/latest/characters/{characterId}/planets/{header.planet_id}?datasource=tranquility&token={accessToken}"));
        //     response.EnsureSuccessStatusCode();
        //     var planetInteraction = JsonSerializer.Deserialize<PlanetaryInteraction>(
        //         await response.Content.ReadAsStringAsync(),
        //         new JsonSerializerOptions
        //         {
        //             PropertyNameCaseInsensitive = true
        //         });
        //     if (planetInteraction == null) throw new Exception("planetInteraction response from Eve Online API is null");
        //     planetInteraction.Header = header;
        //     planetInteractions.Add(planetInteraction);
        // }
        return planetInteractions.ToList();
    }

    public async Task<Planet> GetPlanet(int planetId, string accessToken)
    {
        var planetRepository = await _planetRepository.Get(planetId);
        if (planetRepository is not null) return planetRepository;

        var response = await _httpClientWrapper.GetAsync(new Uri($"https://esi.evetech.net/latest/universe/planets/{planetId}/?datasource=tranquility&token={accessToken}"));
        response.EnsureSuccessStatusCode();
        var planet = JsonSerializer.Deserialize<Planet>(
            await response.Content.ReadAsStringAsync(),
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        if (planet == null) throw new Exception("GetPlanet returned null");

        await _planetRepository.Upsert(planet);
        return planet;
    }

    public async Task<List<Planet>> GetPlanets(List<int> planetIds, string accessToken)
    {
        var planets = new ConcurrentBag<Planet>();
        var planetIdsToPullFromApi = new List<int>();
        var planetsRepository = await _planetRepository.GetMany(planetIds);
        foreach (var planetId in planetIds)
        {
            var planet = planetsRepository.SingleOrDefault(p => p.PlanetId == planetId);
            if (planet is not null) 
            {
                planets.Add(planet);
                continue;
            }
            planetIdsToPullFromApi.Add(planetId);
        }

        var options = new ParallelOptions() {
            MaxDegreeOfParallelism = 5,
        };
        await Parallel.ForEachAsync(planetIdsToPullFromApi, options, async (planetId, _) => {
            var response = await _httpClientWrapper.GetAsync(new Uri($"https://esi.evetech.net/latest/universe/planets/{planetId}/?datasource=tranquility&token={accessToken}"));
            response.EnsureSuccessStatusCode();
            var planet = JsonSerializer.Deserialize<Planet>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            if (planet == null) throw new Exception("GetPlanet returned null");

            await _planetRepository.Upsert(planet);
            planets.Add(planet);
        });

        return planets.ToList();
    }
}