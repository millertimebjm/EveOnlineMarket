using System.Collections.Concurrent;
using System.Text.Json;
using Eve.Models.EveApi;
using Eve.Repositories.Interfaces.Planets;
using Eve.Services.Interfaces.EveApi.Planets;
using Eve.Services.Interfaces.Wrappers;

namespace Eve.Services.EveApi.Planets;

public class PlanetService : IPlanetService
{
    private readonly IHttpClientWrapper _httpClientWrapper;
    private readonly IPlanetRepository _planetRepository;

    public PlanetService(
        IHttpClientWrapper httpClientWrapper,
        IPlanetRepository planetRepository
    )
    {
        _httpClientWrapper = httpClientWrapper;
        _planetRepository = planetRepository;
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