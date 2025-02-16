
using Eve.Mvc.Models;

public interface ITypeRepository
{
    Task<IEnumerable<EveUniverseType>> GetAll();
    Task<EveUniverseType?> Get(int typeId);
    Task<EveUniverseType> Upsert(EveUniverseType type);
    Task<List<EveUniverseType>> GetMarketableTypes();
    Task<List<EveUniverseType>> Search(EveUniverseTypeSearchFilterModel searchFilterModel);
}