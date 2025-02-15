using EveOnlineMarket.Eve.Mvc.Services.Interfaces;

namespace EveOnlineMarket.Eve.Mvc.Services
{
    public class EveOnlineMarketConfigurationService : IEveOnlineMarketConfiguration
    {
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
        public string? CallbackUrl { get; set; }
        public string? ConnectionString { get; set; }

        public string? GetClientId() => ClientId;

        public string? GetClientSecret() => ClientSecret;

        public string? GetCallbackUrl() => CallbackUrl;

        public string? GetConnectionString() => ConnectionString;
    }
}
