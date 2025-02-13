using EveOnlineMarket.Eve.Mvc.Services.Interfaces;

namespace EveOnlineMarket.Eve.Mvc.Services
{
    public class EveOnlineMarketConfigurationService : IEveOnlineMarketConfiguration
    {
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
        public string? CallbackUrl { get; set; }

        public EveOnlineMarketConfigurationService()
        {
            
        }

        public string? GetClientId()
        {
            return ClientId;
        }

        public string? GetClientSecret()
        {
            return ClientSecret;
        }

        public string? GetCallbackUrl()
        {
            return CallbackUrl;
        }
    }
}
