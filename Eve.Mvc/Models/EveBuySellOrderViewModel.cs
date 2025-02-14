namespace Eve.Mvc.Models 
{
    public class GetBuySellOrdersViewModel
    {
        public User User { get; set; }
        public EveMarketOrder MarketOrders { get; set; }
        public Task<List<long>> UserOrderIds { get; set; }
    }
}
