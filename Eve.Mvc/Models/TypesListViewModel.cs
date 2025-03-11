using Eve.Models.EveApi;
using Eve.Models;
using Eve.Models.EveTypes;

namespace Eve.Mvc.Models;

public class TypesListViewModel
{
    public EveTypeSearchFilterModel SearchFilterModel { get; set; } 
        = new EveTypeSearchFilterModel();
    public List<EveType> EveTypes { get; set; } 
        = new List<EveType>();
    public Task<EveType?> TypeTask { get; set; } 
        = Task.FromResult(default(EveType));
}