using Eve.Repositories.Interfaces.Types;
using Eve.Services.Interfaces.EveApi.EveTypes;
using Eve.Services.Interfaces.EveApi.RefreshTypes;

namespace Eve.Services.EveApi.RefreshServices;

public class RefreshTypesService : IRefreshTypes
{
    private readonly ITypeRepository _typeRepository;
    private readonly IEveTypeService _eveTypeService;
    public RefreshTypesService(
        ITypeRepository typeRepository,
        IEveTypeService eveTypeService)
    {
        _typeRepository = typeRepository;
        _eveTypeService = eveTypeService;
    }

    public async Task RefreshTypes(string accessToken) 
    {
        var databaseTypes = await _typeRepository.GetAll();
        var databaseTypesHashSet = databaseTypes.Select(t => t.TypeId).ToHashSet();
        await foreach (var typeId in _eveTypeService.GetEveTypeIds(accessToken))
        {
            var databaseType = databaseTypesHashSet.SingleOrDefault(t => t == typeId);
            if (typeId == 0 || databaseType > 0) continue;
            await Task.Delay(100);
            try
            {
                var type = await _eveTypeService.GetEveType(typeId, accessToken);
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