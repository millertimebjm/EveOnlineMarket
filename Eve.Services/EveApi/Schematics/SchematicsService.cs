using System.Collections.Concurrent;
using System.Text.Json;
using Eve.Models.EveApi;
using Eve.Repositories.Interfaces.Schematics;
using Eve.Services.Interfaces.EveApi.Schematics;
using Eve.Services.Interfaces.Wrappers;
using Eve.Services.Wrappers;

namespace Eve.Services.EveApi.Schematics;

public class SchematicService(
    ISchematicsRepository _schematicsRepository,
    IHttpClientWrapper _httpClientWrapper) : ISchematicsService
{

    public async Task<Schematic> Get(int schematicId, string accessToken)
    {
        var schematicRepository = await _schematicsRepository.Get(schematicId);
        if (schematicRepository != null) return schematicRepository;

        var response = await _httpClientWrapper.GetAsync(new Uri($"https://esi.evetech.net/latest/universe/schematics/{schematicId}/?datasource=tranquility&token={accessToken}"));
        response.EnsureSuccessStatusCode();
        var schematic = JsonSerializer.Deserialize<Schematic>(
            await response.Content.ReadAsStringAsync(),
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        if (schematic == null) throw new Exception("GetSchematic returned null");
        schematic.SchematicId = schematicId;

        await _schematicsRepository.Upsert(schematic);
        return schematic;
    }

    public async Task<List<Schematic>> GetAll(List<int> schematicIds, string accessToken)
    {
        var schematicRepositories = await _schematicsRepository.GetAll(schematicIds);
        if (schematicRepositories.Count() == schematicIds.Count()) return schematicRepositories;

        var schematicsBag = new ConcurrentBag<Schematic>();
        await Parallel.ForEachAsync(schematicIds.Where(si => !schematicRepositories.Any(sr => sr.SchematicId == si)), async (schematicId, token) => {
            var response = await _httpClientWrapper.GetAsync(new Uri($"https://esi.evetech.net/latest/universe/schematics/{schematicId}/?datasource=tranquility&token={accessToken}"));
            response.EnsureSuccessStatusCode();

            var schematic = JsonSerializer.Deserialize<Schematic>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            if (schematic == null) throw new Exception("GetSchematic returned null");
            schematic.SchematicId = schematicId;
            await _schematicsRepository.Upsert(schematic);
            schematicsBag.Add(schematic);
        });

        schematicRepositories.AddRange(schematicsBag);
        return schematicRepositories;
    }

    public async Task<Schematic> Upsert(Schematic schematic, string accessToken)
    {
        await Task.Delay(1000);
        throw new NotImplementedException();
    }
}