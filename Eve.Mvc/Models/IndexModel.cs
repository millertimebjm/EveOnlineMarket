
namespace Eve.Mvc.Models;

public class IndexModel
{
    public Task<List<EveMarketOrder>> EveMarketOrdersTask {get; set;}
    public User User {get; set;}
    public Task<Dictionary<int, EveUniverseType>> Types { get; set; }
}