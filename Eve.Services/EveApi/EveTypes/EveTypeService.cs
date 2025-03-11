using System.Text.Json;
using Eve.Models.EveApi;
using Eve.Models.EveTypes;
using Eve.Repositories.Interfaces.Types;
using Eve.Services.Interfaces.EveApi.EveTypes;
using Eve.Services.Interfaces.Wrappers;

namespace Eve.Services.EveApi.EveTypes;

public class EveTypeService : IEveTypeService
{
    private readonly ITypeRepository _eveTypeRepository;
    private readonly IHttpClientWrapper _httpClientWrapper;
    public EveTypeService(
        ITypeRepository eveTypeRepository,
        IHttpClientWrapper httpClientWrapper)
    {
        _eveTypeRepository = eveTypeRepository;
        _httpClientWrapper = httpClientWrapper;
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

    public async Task<EveType> GetEveType(
        int typeId,
        string accessToken)
    {
        var response = await _httpClientWrapper.GetAsync(new Uri($"https://esi.evetech.net/latest/universe/types/{typeId}/?datasource=tranquility&token={accessToken}"));
        response.EnsureSuccessStatusCode();
        var type = JsonSerializer.Deserialize<EveType>(
            await response.Content.ReadAsStringAsync(),
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        if (type is null) throw new Exception("type response from Eve Online API is null or empty");
        return type;
    }
    
    public async Task<List<EveType>> Search(EveTypeSearchFilterModel model)
    {
        return await _eveTypeRepository.Search(model);
    }
}