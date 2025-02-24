using Eve.Models.EveApi;
using Eve.Models.Users;

namespace Eve.Mvc.Models;

public class GetBuySellOrdersViewModel
{
    public User User { get; set; } = new User();
    public Task<List<Order>>? MarketOrdersTask { get; set; }
    public Task<List<long>>? UserOrderIdsTask { get; set; }
    public Task<EveType?>? TypesTask { get; set; }
}