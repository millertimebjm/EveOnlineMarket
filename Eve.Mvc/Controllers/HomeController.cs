using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EveOnlineMarket.Models;
using System.Web;
using Eve.Mvc.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Eve.Services.Interfaces.Authentications;
using Eve.Repositories.Interfaces.Users;
using Eve.Services.Interfaces.EveApi;
using Eve.Repositories.Interfaces.Types;
using Eve.Models.Users;
using Eve.Models.EveApi;
using Eve.Models;
using Eve.Configurations;
using Eve.Services.Interfaces.EveApi.EveTypes;
using Eve.Services.Interfaces.Orders;
using Eve.Services.Interfaces.EveApi.Characters;

namespace Eve.Mvc.Controllers;

public class HomeController : BaseController
{
    private readonly ILogger<HomeController> _logger;
    private readonly IAuthenticationService _authenticationService;
    private readonly IUserRepository _userRepository;
    private readonly IEveApi _eveApiService;
    private readonly EveOnlineMarketConfigurationService _configuration;
    private readonly ITypeRepository _typeRepository;
    private readonly IEveTypeService _eveTypeService;
    private readonly IOrdersService _ordersService;
    private readonly ICharacterService _characterService;

    public HomeController(
        IAuthenticationService authenticationService,
        IUserRepository userRepository,
        IEveApi eveApiService,
        IOptionsSnapshot<EveOnlineMarketConfigurationService> options,
        ILogger<HomeController> logger,
        ITypeRepository typeRepository,
        IEveTypeService eveTypeService,
        IOrdersService ordersService,
        ICharacterService characterService) : base(
            eveApiService,
            userRepository,
            options,
            authenticationService)
    {
        _authenticationService = authenticationService;
        _userRepository = userRepository;
        _eveApiService = eveApiService;
        _logger = logger;
        _configuration = options.Value;
        _typeRepository = typeRepository;
        _eveTypeService = eveTypeService;
        _ordersService = ordersService;
        _characterService = characterService;
    }

    public async Task<IActionResult> Index()
    {
        var user = await GetUser();
        //if (DateTime.UtcNow > user.TokenExpirationDate) await RefreshToken(user);
        if (user == null) return Redirect("/login");

        var model = new IndexViewModel
        {
            User = user,
        };

        model.EveMarketOrdersTask = _ordersService.GetMarketOrders(
            model.User.UserId,
            model.User.AccessToken);

        model.Types = GetTypes(model.EveMarketOrdersTask, model.User.AccessToken);
        return View(model);
    }

    private async Task<Dictionary<int, EveType>> GetTypes(
        Task<List<Order>> marketOrders,
        string accessToken)
    {
        Dictionary<int, EveType> types = new();
        List<EveType> typeTasks = new();
        foreach (var typeId in (await marketOrders).Select(mo => mo.TypeId).Distinct())
        {
            types.Add(typeId, await GetType(typeId, accessToken));
        }
        return types;
    }

    private async Task<EveType> GetType(int typeId, string accessToken)
    {
        var type = await _typeRepository.Get(typeId);
        if (type != null) return type;
        type = await _eveTypeService.GetEveType(typeId, accessToken);
        type = await _typeRepository.Upsert(type);
        return type;
    }

    [Route("login")]
    public IActionResult Login()
    {
        var clientId = _configuration.GetClientId();
        if (string.IsNullOrEmpty(clientId)) throw new ArgumentNullException(nameof(clientId));

        var callback = _configuration.GetCallbackUrl();
        if (string.IsNullOrEmpty(clientId)) throw new ArgumentNullException(nameof(callback));
        Console.WriteLine(callback);

        var state = HttpUtility.UrlEncode(Guid.NewGuid().ToString());
        var scopes = HttpUtility.UrlEncode("esi-markets.read_character_orders.v1 esi-planets.manage_planets.v1 esi-planets.read_customs_offices.v1");
        return Redirect($"https://login.eveonline.com/v2/oauth/authorize?response_type=code&client_id={clientId}&redirect_uri={callback ?? ""}&state={state}&scope={scopes}");
    }

    [Route("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.SetString(SessionUserId, "");
        return Redirect("/login");
    }

    [Route("Home/GetBuySellOrders/{typeId?}")]
    public async Task<IActionResult> GetBuySellOrders(int? typeId)
    {
        var user = await GetUser();
        if (user == null) return Redirect("/login");

        var eveTypes = typeId.HasValue && typeId.Value > 0 
            ? new List<EveType>{await _eveTypeService.GetEveType(typeId.Value, user.AccessToken)}
            : await _typeRepository.Search(new TypeSearchFilterModel());

        var model = new TypesListViewModel()
        {
            SearchFilterModel = new TypeSearchFilterModel(),
            EveTypes = eveTypes,
        };
        var typesModel = new TypesViewModel()
        {
            TypesListPartialTask = this.RenderPartialViewToStringAsync("TypesList", model),
            EveTypes = eveTypes,
        };

        return View(typesModel);
    }

    public async Task<IActionResult> GetBuySellOrdersList(int typeId)
    {
        var user = await GetUser();
        if (user == null) return Redirect("/login");

        var eveMarketOrdersTask = _ordersService.GetBuySellOrders(typeId, user.AccessToken);

        var model = new GetBuySellOrdersViewModel()
        {
            User = user,
            MarketOrdersTask = eveMarketOrdersTask,
            UserOrderIdsTask = _ordersService.GetMarketOrderIds(
                user.UserId,
                user.AccessToken),
            TypesTask = _typeRepository.Get(typeId),
        };
        return PartialView(model);
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
        if (authenticationResponseModel == null) throw new Exception();
        user.AccessToken = authenticationResponseModel.AccessToken;
        user.BearerToken = authenticationResponseModel.RefreshToken;
        user.TokenGrantedDateTime = authenticationResponseModel.IssuedDate;
        user.TokenExpirationDate = authenticationResponseModel.IssuedDate.AddSeconds(authenticationResponseModel.ExpiresIn);

        user.UserId = await _characterService.GetCharacterId(
            user.AccessToken
        );
        HttpContext.Session.SetString(SessionUserId, user.UserId.ToString());

        await _userRepository.Upsert(user);

        return Redirect("/");
    }

    public async Task<IActionResult> Types()
    {
        var model = new TypesListViewModel()
        {
            SearchFilterModel = new TypeSearchFilterModel(),
            EveTypes = await _typeRepository.Search(new TypeSearchFilterModel()),
        };
        var typesModel = new TypesViewModel()
        {
            TypesListPartialTask = this.RenderPartialViewToStringAsync("TypesList", model),
        };
        return View(typesModel);
    }

    public async Task<IActionResult> TypesList(TypeSearchFilterModel searchFilterModel)
    {
        var model = new TypesListViewModel()
        {
            SearchFilterModel = searchFilterModel,
            EveTypes = await _typeRepository.Search(searchFilterModel),
        };
        return PartialView(model);
    }

    public async Task<IActionResult> PlanetaryInteractions()
    {
        var user = await GetUser();
        if (user == null) return Redirect("/login");

        var planetaryInteractionsTask = _eveApiService.GetPlanetaryInteractions(user.UserId, user.AccessToken);
        var typesList = new List<int>();
        typesList.AddRange((await planetaryInteractionsTask).SelectMany(pi => pi.pins.Select(p => p.type_id)));
        typesList.AddRange((await planetaryInteractionsTask).SelectMany(pi => pi.routes.Select(r => r.content_type_id)));
        var model = new PlanetaryInteractionsViewModel() 
        {
            PlanetaryInteractionsTask = planetaryInteractionsTask,
            PlanetsTask = _eveApiService.GetPlanets((await planetaryInteractionsTask).Select(pi => pi.Header?.planet_id ?? 0).ToList(), user.AccessToken),
            //TypesTask = _typeRepository.GetMany(typesList),
        };
        return View(model);
    }
}

public static class ControllerExtensions
{
    public static async Task<string> RenderPartialViewToStringAsync<TModel>(
        this Controller controller,
        string viewName,
        TModel model)
    {
        if (string.IsNullOrEmpty(viewName))
            viewName = controller.ControllerContext.ActionDescriptor.ActionName;

        controller.ViewData.Model = model;

        using (var writer = new StringWriter())
        {
            var viewEngine = controller.HttpContext.RequestServices
                .GetService<ICompositeViewEngine>() as ICompositeViewEngine;

            if (viewEngine == null) throw new Exception("viewEngine is null");
            var viewResult = viewEngine.FindView(controller.ControllerContext, viewName, false);

            if (!viewResult.Success)
                return $"A view with the name '{viewName}' could not be found";

            var viewContext = new ViewContext(
                controller.ControllerContext,
                viewResult.View,
                controller.ViewData,
                controller.TempData,
                writer,
                new HtmlHelperOptions()
            );

            await viewResult.View.RenderAsync(viewContext);

            return writer.GetStringBuilder().ToString();
        }
    }
}