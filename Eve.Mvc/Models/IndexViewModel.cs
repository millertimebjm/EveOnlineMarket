using Eve.Models.EveApi;
using Eve.Models.Users;

namespace Eve.Mvc.Models;

public class IndexViewModel
{
    public Task<List<Order>> EveMarketOrdersTask {get; set;} 
        = Task.FromResult(new List<Order>());
    public User User {get; set;} 
        = new User();
    public Task<Dictionary<int, EveType>> TypesTask { get; set; } 
        = Task.FromResult(new Dictionary<int, EveType>());

    public Task<IDictionary<long, int>> OrderRanksTask { get; set; }
        = Task.FromResult((IDictionary<long, int>)new Dictionary<long, int>());
}