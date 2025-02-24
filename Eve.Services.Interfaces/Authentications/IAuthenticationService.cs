using Eve.Models.EveApi;

namespace Eve.Services.Interfaces.Authentications;

public interface IAuthenticationService
{
    Task<Authentication> GetAccessToken(
        string userId, 
        string clientId,
        string clientSecret);

    Task<Authentication> RefreshAccessToken(
        string clientId,
        string clientSecret,
        string refreshToken);
}