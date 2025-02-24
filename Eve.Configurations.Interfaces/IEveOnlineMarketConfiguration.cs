namespace Eve.Configurations.Interfaces;

public interface IEveOnlineMarketConfiguration
{
    string? GetClientId();
    string? GetClientSecret();
    string? GetCallbackUrl();
    string? GetConnectionString();
}