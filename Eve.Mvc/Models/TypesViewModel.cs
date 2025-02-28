
using Eve.Models.EveApi;

namespace Eve.Mvc.Models;

public class TypesViewModel
{
    public Task<string>? TypesListPartialTask { get; set; }
    public List<EveType> EveTypes { get; set; } = new List<EveType>();
}