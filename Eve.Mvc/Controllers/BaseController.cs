
using Eve.Configurations;
using Eve.Models.Users;
using Eve.Repositories.Interfaces.Users;
using Eve.Services.Interfaces.Authentications;
using Eve.Services.Interfaces.EveApi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Eve.Mvc.Controllers;

public class BaseController : Controller
{
    protected const string SessionUserId = "_UserId";
    private readonly IUserRepository _userRepository;
    private readonly EveOnlineMarketConfigurationService _configuration;
    private readonly IAuthenticationService _authenticationService;

    public BaseController(
        IUserRepository userRepository,
        IOptionsSnapshot<EveOnlineMarketConfigurationService> options,
        IAuthenticationService authenticationService)
    {
        _userRepository = userRepository;
        _configuration = options.Value;
        _authenticationService = authenticationService;
    }

    protected async Task<User?> GetUser()
    {
        var userIdString = HttpContext.Session.GetString(SessionUserId);
        if (string.IsNullOrEmpty(userIdString))
        {
            return null;
        }
        var user = await _userRepository.Get(long.Parse(userIdString));
        if (user == null) throw new Exception("");

        if (DateTime.UtcNow > user.TokenExpirationDate)
        {
            var clientId = _configuration.GetClientId();
            if (string.IsNullOrEmpty(clientId)) throw new ArgumentNullException(nameof(clientId));

            var clientSecret = _configuration.GetClientSecret();
            if (string.IsNullOrEmpty(clientSecret)) throw new ArgumentNullException(nameof(clientSecret));
            var authenticationResponseModel = await _authenticationService.RefreshAccessToken(
                clientId,
                clientSecret,
                user.BearerToken);
            user.AccessToken = authenticationResponseModel.AccessToken;
            user.BearerToken = authenticationResponseModel.RefreshToken;
            user.TokenGrantedDateTime = authenticationResponseModel.IssuedDate;
            user.TokenExpirationDate = authenticationResponseModel.IssuedDate.AddSeconds(authenticationResponseModel.ExpiresIn);
            await _userRepository.Upsert(user);
        }
        return user;
    }
}