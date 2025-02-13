namespace EveOnlineMarket.Eve.Mvc.Services.Interfaces
{
    public interface IEveOnlineMarketConfiguration 
    {
        string? GetClientId();
        string? GetClientSecret();
        string? GetCallbackUrl();
    }
}