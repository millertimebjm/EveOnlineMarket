namespace Eve.Mvc.Models
{
    public class GetBuySellOrdersViewModel
    {
        public User? User { get; set; }
        public Task<List<EveMarketOrder>>? MarketOrdersTask { get; set; }
        public Task<List<long>>? UserOrderIdsTask { get; set; }
        public Task<EveUniverseType?>? TypesTask { get; set; }
    }
}
