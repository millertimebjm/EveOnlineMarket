
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
}