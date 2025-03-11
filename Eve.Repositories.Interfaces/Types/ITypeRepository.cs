using Eve.Models.EveApi;
using Eve.Models.EveTypes;

namespace Eve.Repositories.Interfaces.Types;

public interface ITypeRepository
{
    Task<IEnumerable<EveType>> GetAll();
    Task<EveType?> Get(int typeId);
    Task<EveType> Upsert(EveType type);
    Task<List<EveType>> GetMarketableTypes();
    Task<List<EveType>> Search(EveTypeSearchFilterModel model);
}