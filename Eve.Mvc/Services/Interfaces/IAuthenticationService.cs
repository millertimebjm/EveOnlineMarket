
using Eve.Mvc.Models;

namespace Eve.Mvc.Services.Interfaces;

public interface IAuthenticationService
{
    Task<AuthenticationResponseModel> GetAccessToken(
        string userId, 
        string clientId,
        string clientSecret);

    Task<AuthenticationResponseModel> RefreshAccessToken(
        string clientId,
        string clientSecret,
        string refreshToken);
}