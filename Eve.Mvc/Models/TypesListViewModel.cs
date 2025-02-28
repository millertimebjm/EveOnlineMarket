using Eve.Models.EveApi;
using Eve.Models;

namespace Eve.Mvc.Models;

public class TypesListViewModel
{
    public TypeSearchFilterModel SearchFilterModel { get; set; } = new TypeSearchFilterModel();
    public List<EveType> EveTypes { get; set; } = new List<EveType>();
    public Task<EveType?> TypeTask { get; set; }
}