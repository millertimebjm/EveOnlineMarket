
namespace Eve.Mvc.Models;

public class TypesListViewModel
{
    public EveUniverseTypeSearchFilterModel searchFilterModel { get; set; } = new EveUniverseTypeSearchFilterModel();
    public List<EveUniverseType> eveUniverseTypes { get; set; } = new List<EveUniverseType>();
}