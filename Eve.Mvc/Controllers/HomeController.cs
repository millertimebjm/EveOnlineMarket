using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EveOnlineMarket.Models;
using Eve.Mvc.Services.Interfaces;
using System.Web;
using Eve.Mvc.Models;
using EveOnlineMarket.Eve.Mvc.Services.Interfaces;
using EveOnlineMarket.Eve.Mvc.Services;
using Microsoft.Extensions.Options;

namespace EveOnlineMarket.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IAuthenticationService _authenticationService;
    private readonly IUserRepository _userRepository;
    private readonly IEveApi _eveApiService;
    private const string SessionUserId = "_UserId";
    private readonly IEveOnlineMarketConfiguration _configuration;

    public HomeController(
        IAuthenticationService authenticationService,
        IUserRepository userRepository,
        IEveApi eveApiService,
        IOptionsSnapshot<EveOnlineMarketConfigurationService> options,
        ILogger<HomeController> logger)
    {
        _authenticationService = authenticationService;
        _userRepository = userRepository;
        _eveApiService = eveApiService;
        _logger = logger;
        _configuration = options.Value;
    }

    public async Task<IActionResult> Index()
    {
        var clientId = _configuration.GetClientId();
        if (string.IsNullOrEmpty(clientId)) throw new ArgumentNullException(nameof(clientId));

        var clientSecret = _configuration.GetClientSecret();
        if (string.IsNullOrEmpty(clientSecret)) throw new ArgumentNullException(nameof(clientSecret));

        var userIdString = HttpContext.Session.GetString(SessionUserId);
        if (string.IsNullOrEmpty(userIdString))
        {
            return Redirect("/login");
        }
        var model = new IndexModel
        {
            User = await _userRepository.Get(int.Parse(userIdString))
        };

        if (DateTime.UtcNow > model.User.TokenExpirationDate)
        {
            var authenticationResponseModel = await _authenticationService.RefreshAccessToken(
                clientId,
                clientSecret,
                model.User.BearerToken);
            model.User.AccessToken = authenticationResponseModel.AccessToken;
            model.User.BearerToken = authenticationResponseModel.RefreshToken;
            model.User.TokenGrantedDateTime = authenticationResponseModel.IssuedDate;
            model.User.TokenExpirationDate = authenticationResponseModel.IssuedDate.AddSeconds(authenticationResponseModel.ExpiresIn);
            await _userRepository.Upsert(model.User);
        }

        model.EveMarketOrdersTask = _eveApiService.GetMarketOrders(
            model.User.UserId, 
            model.User.AccessToken);

        model.Types = GetTypes(model.EveMarketOrdersTask, model.User.AccessToken);
        return View(model);
    }

    private async Task<Dictionary<int, EveUniverseType>> GetTypes(
        Task<List<EveMarketOrder>> marketOrders,
        string accessToken)
    {
        Dictionary<int, EveUniverseType> types = new();
        List<Task<EveUniverseType>> typeTasks = new();
        foreach (var typeId in (await marketOrders).Select(mo => mo.TypeId).Distinct())
        {
            typeTasks.Add(GetType(typeId, accessToken));
        }
        await Task.WhenAll(typeTasks);
        foreach (var typeTask in typeTasks)
        {
            types.Add(typeTask.Result.TypeId, typeTask.Result);
        }
        return types;
    }

    private async Task<EveUniverseType> GetType(int typeId, string accessToken)
    {
        return await _eveApiService.GetUniverseType(typeId, accessToken);
    }

    [Route("login")]
    public IActionResult Login()
    {
        var clientId = _configuration.GetClientId();
        if (string.IsNullOrEmpty(clientId)) throw new ArgumentNullException(nameof(clientId));

        var callback = _configuration.GetCallbackUrl();
        if (string.IsNullOrEmpty(clientId)) throw new ArgumentNullException(nameof(callback));

        var state = HttpUtility.UrlEncode(Guid.NewGuid().ToString());
        var scopes = HttpUtility.UrlEncode("esi-markets.read_character_orders.v1");
        return Redirect($"https://login.eveonline.com/v2/oauth/authorize?response_type=code&client_id={clientId}&redirect_uri={callback}&state={state}&scope={scopes}");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [Route("callback")]
    public async Task<IActionResult> Callback(string code, string state)
    {
        var clientId = _configuration.GetClientId();
        if (string.IsNullOrEmpty(clientId)) throw new ArgumentNullException(nameof(clientId));

        var clientSecret = _configuration.GetClientSecret();
        if (string.IsNullOrEmpty(clientSecret)) throw new ArgumentNullException(nameof(clientSecret));

        var user = new User
        {
            AuthorizationCode = code
        };
        var authenticationResponseModel = await _authenticationService.GetAccessToken(
            user.AuthorizationCode,
            clientId,
            clientSecret);
        user.AccessToken = authenticationResponseModel.AccessToken;
        user.BearerToken = authenticationResponseModel.RefreshToken;
        user.TokenGrantedDateTime = authenticationResponseModel.IssuedDate;
        user.TokenExpirationDate = authenticationResponseModel.IssuedDate.AddSeconds(authenticationResponseModel.ExpiresIn);
        
        user.UserId = await _eveApiService.GetCharacterId(
            user.AccessToken
        );
        HttpContext.Session.SetString(SessionUserId, user.UserId.ToString());
        
        await _userRepository.Upsert(user);

        return Redirect("/");
    }
}
