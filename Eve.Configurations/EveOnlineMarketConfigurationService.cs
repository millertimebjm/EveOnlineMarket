using Eve.Configurations.Interfaces;

namespace Eve.Configurations;

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