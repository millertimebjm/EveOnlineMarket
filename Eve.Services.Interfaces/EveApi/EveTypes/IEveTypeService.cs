using Eve.Models.EveApi;
using Eve.Models.EveTypes;

namespace Eve.Services.Interfaces.EveApi.EveTypes;

public interface IEveTypeService
{
    Task<EveType> GetEveType(
        int typeId, 
        string accessToken);
    IAsyncEnumerable<int> GetEveTypeIds(
        string accessToken);

    Task<List<EveType>> Search(EveTypeSearchFilterModel model);
}