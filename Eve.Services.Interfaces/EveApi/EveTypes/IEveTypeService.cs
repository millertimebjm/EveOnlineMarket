using Eve.Models.EveApi;

namespace Eve.Services.Interfaces.EveApi.EveTypes;

public interface IEveTypeService
{
    Task<EveType> GetEveType(
        int typeId, 
        string accessToken);
    IAsyncEnumerable<int> GetEveTypeIds(
        string accessToken);
}