using Eve.Models.EveApi;
using Eve.Models.Users;

namespace Eve.Mvc.Models;

public class IndexViewModel
{
    public Task<List<Order>>? EveMarketOrdersTask {get; set;}
    public User User {get; set;} = new User();
    public Task<Dictionary<int, EveType>>? Types { get; set; }
}