using Eve.Repositories.Interfaces.Types;
using Eve.Services.Interfaces.EveApi;
using Eve.Services.Interfaces.EveApi.RefreshTypes;

namespace Eve.Services.EveApi.RefreshServices;

public class RefreshTypesService : IRefreshTypes
{
    private readonly ITypeRepository _typeRepository;
    private readonly IEveApi _eveApiService;
    public RefreshTypesService(
        ITypeRepository typeRepository,
        IEveApi eveApiService)
    {
        _typeRepository = typeRepository;
        _eveApiService = eveApiService;
    }

    public async Task RefreshTypes(string accessToken) 
    {
        var databaseTypes = await _typeRepository.GetAll();
        var databaseTypesHashSet = databaseTypes.Select(t => t.TypeId).ToHashSet();
        await foreach (var typeId in _eveApiService.GetEveTypeIds(accessToken))
        {
            //var type = await _typeRepository.Get(typeId);
            var databaseType = databaseTypesHashSet.SingleOrDefault(t => t == typeId);
            if (typeId == 0 || databaseType > 0) continue;
            await Task.Delay(100);
            try
            {
                var type = await _eveApiService.GetEveType(typeId, accessToken);
                await _typeRepository.Upsert(type);
            }
            catch
            {
                await Task.Delay(10 * 1000);
                continue;
            }
        }
    }
}