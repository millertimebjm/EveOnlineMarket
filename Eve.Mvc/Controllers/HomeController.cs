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
using Eve.Repositories.Interfaces.Types;
using Eve.Models.Users;
using Eve.Models.EveApi;
using Eve.Configurations;
using Eve.Services.Interfaces.EveApi.EveTypes;
using Eve.Services.Interfaces.Orders;
using Eve.Services.Interfaces.EveApi.Characters;
using Eve.Services.Interfaces.EveApi.Planets;
using Eve.Models.EveTypes;
using Eve.Services.Interfaces.EveApi.Schematics;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Concurrent;
using Eve.Mvc.Services;

namespace Eve.Mvc.Controllers;

public class HomeController : BaseController
{
    private readonly ILogger<HomeController> _logger;
    private readonly IAuthenticationService _authenticationService;
    private readonly IUserRepository _userRepository;
    private readonly IPlanetService _planetService;
    private readonly EveOnlineMarketConfigurationService _configuration;
    private readonly ITypeRepository _typeRepository;
    private readonly IEveTypeService _eveTypeService;
    private readonly IOrdersService _ordersService;
    private readonly ICharacterService _characterService;
    private readonly ISchematicsService _schematicsService;
    private readonly IDistributedCacheWrapper _distributedCache;

    public HomeController(
        IAuthenticationService authenticationService,
        IUserRepository userRepository,
        IPlanetService planetService,
        IOptionsSnapshot<EveOnlineMarketConfigurationService> options,
        ILogger<HomeController> logger,
        ITypeRepository typeRepository,
        IEveTypeService eveTypeService,
        IOrdersService ordersService,
        ICharacterService characterService,
        ISchematicsService schematicsService,
        IDistributedCacheWrapper distributedCache) : base(
            userRepository,
            options,
            authenticationService)
    {
        _authenticationService = authenticationService;
        _userRepository = userRepository;
        _planetService = planetService;
        _logger = logger;
        _configuration = options.Value;
        _typeRepository = typeRepository;
        _eveTypeService = eveTypeService;
        _ordersService = ordersService;
        _characterService = characterService;
        _schematicsService = schematicsService;
        _distributedCache = distributedCache;
    }

    public async Task<IActionResult> Index()
    {
        var user = await GetUser();
        if (user == null) return Redirect("/login");

        var model = new IndexViewModel
        {
            User = user,
        };

        model.EveMarketOrdersTask = _ordersService.GetMarketOrders(
            model.User.UserId,
            model.User.AccessToken);

        IDictionary<int, long> typeOrderIds = (await model.EveMarketOrdersTask)
            .Select(mo => new KeyValuePair<int, long>(mo.TypeId, mo.OrderId))
            .ToDictionary(pair => pair.Key, pair => pair.Value);
        model.OrderRanksTask = _ordersService.GetMarketOrderRanks(
            typeOrderIds,
            model.User.AccessToken
        );

        model.TypesTask = GetTypes(model.EveMarketOrdersTask, model.User.AccessToken);
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
            : await _typeRepository.Search(new EveTypeSearchFilterModel());

        var model = new TypesListViewModel()
        {
            SearchFilterModel = new EveTypeSearchFilterModel(),
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
            SearchFilterModel = new EveTypeSearchFilterModel(),
            EveTypes = await _typeRepository.Search(new EveTypeSearchFilterModel()),
        };
        var typesModel = new TypesViewModel()
        {
            TypesListPartialTask = this.RenderPartialViewToStringAsync("TypesList", model),
        };
        return View(typesModel);
    }

    public async Task<IActionResult> TypesList(EveTypeSearchFilterModel searchFilterModel)
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

        var planetaryInteractionsTask = _planetService.GetPlanetaryInteractions(user.UserId, user.AccessToken);
        var typesList = new List<int>();
        typesList.AddRange((await planetaryInteractionsTask).SelectMany(pi => pi.pins.Select(p => p.type_id)));
        typesList.AddRange((await planetaryInteractionsTask).SelectMany(pi => pi.routes.Select(r => r.content_type_id)));
        typesList = typesList.Distinct().ToList();
        var schematicsIds = (await planetaryInteractionsTask).SelectMany(pi => pi.pins.Select(p => p.schematic_id));
        var model = new PlanetaryInteractionsViewModel() 
        {
            PlanetaryInteractionsTask = planetaryInteractionsTask,
            PlanetsTask = _planetService.GetPlanets((await planetaryInteractionsTask).Select(pi => pi.Header?.planet_id ?? 0).ToList(), user.AccessToken),
            TypesTask = _typeRepository.Search(new EveTypeSearchFilterModel()
            { 
                TypeIds = typesList.ToHashSet(), 
                SchematicsIds = schematicsIds.ToHashSet(),
                IsMarketableType = false, 
                Take = 100, 
            }),
        };
        model.SchematicsListTask = _schematicsService.GetAll((await planetaryInteractionsTask).SelectMany(pi => pi.pins.Select(p => p.schematic_id)).Where(p => p > 0).Distinct().ToList(), user.AccessToken);
        model.BestBuyOrderValueByTypeTask = GetBestBuyOrderValueByTypeTask((await model.TypesTask).Select(t => t.TypeId), user);

        return View(model);
    }

    private async Task<IDictionary<int, decimal>> GetBestBuyOrderValueByTypeTask(IEnumerable<int> typeIds, User user)
    {
        IDictionary<int, decimal> bestBuyOrderValueByTypeDictionary = new ConcurrentDictionary<int, decimal>();
        await Parallel.ForEachAsync(typeIds, async (typeId, token) => {
            var value = await GetBestBuyOrderValueByTypeWithCache(typeId, user);
            bestBuyOrderValueByTypeDictionary.Add(typeId, value);
        });
        return bestBuyOrderValueByTypeDictionary;
    }

    private async Task<decimal> GetBestBuyOrderValueByTypeWithCache(int typeId, User user)
    {
        var cacheKey = $"{DistributedCacheTypeEnum.BuySellOrdersByType}-{typeId}";
        var orders = await _distributedCache.GetAsync<List<Order>>(cacheKey);
        
        if (orders == null)
        {
            orders = await _ordersService.GetBuySellOrders(typeId, user.AccessToken);
            await _distributedCache.SetAsync(cacheKey, orders);
        }
        var order = orders
            .Where(o => o.IsBuyOrder)
            .OrderByDescending(o => o.Price)
            .FirstOrDefault();
        return order?.Price ?? 0m;
    }

    public async Task<IActionResult> BillOfMaterials()
    {
        var user = await GetUser();
        if (user == null) return Redirect("/login");

        return View();
    }

    public async Task<JsonResult> EveTypeSearch(HashSet<int>? ids, string keyword)
    {
        var user = await GetUser();
        if (user == null) throw new Exception("You are not logged in.");

        if (ids is null && string.IsNullOrWhiteSpace(keyword)) return Json(new List<EveType>());

        EveTypeSearchFilterModel eveTypeSearchFilterModel = new()
        {
            TypeIds = ids ?? new HashSet<int>(),
            Keyword = keyword,
            Take = 20,
            Skip = 0,
        };
        var eveTypes = await _eveTypeService.Search(eveTypeSearchFilterModel);
        return Json(eveTypes);
    }

    public async Task<JsonResult> GetMarketOrder(int typeId, bool isBuyOrder = true)
    {
        var user = await GetUser();
        if (user == null) throw new Exception("You are not logged in.");

        var marketOrders = await _ordersService.GetBuySellOrders(typeId, user.AccessToken);
        marketOrders = marketOrders.Where(mo => isBuyOrder == mo.IsBuyOrder).ToList();
        Order? marketOrder;
        if (isBuyOrder) marketOrder = marketOrders.OrderByDescending(mo => mo.Price).FirstOrDefault();
        else marketOrder = marketOrders.OrderBy(mo => mo.Price).FirstOrDefault();
        return Json(marketOrder);
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