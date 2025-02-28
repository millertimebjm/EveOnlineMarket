using Eve.Configurations;
using Eve.Repositories.Interfaces.Users;
using Eve.Services.Interfaces.Authentications;
using Eve.Services.Interfaces.EveApi;
using Eve.Services.Interfaces.EveApi.RefreshTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Eve.Mvc.Controllers;

public class RefreshController : BaseController
{
    private readonly IRefreshTypes _refreshTypesService;
    public RefreshController(
        IRefreshTypes refreshTypesService,
        IEveApi eveApiService,
        IUserRepository userRepository,
        IOptionsSnapshot<EveOnlineMarketConfigurationService> options,
        IAuthenticationService authenticationService) : base(
            eveApiService, userRepository, options, authenticationService
        )
    {
        _refreshTypesService = refreshTypesService;
    }

    public async Task<IActionResult> Types()
    {
        var user = await GetUser();
        if (user == null) return Redirect("/login");

        await _refreshTypesService.RefreshTypes(user.AccessToken);

        return Accepted();
    }
}