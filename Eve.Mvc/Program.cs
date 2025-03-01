using Eve.Services.Interfaces.Authentications;
using Eve.Services.Authentications;
using Eve.Repositories.Interfaces.Users;
using Eve.Repositories.Users;
using Eve.Services.Interfaces.EveApi;
using Eve.Services.EveApi;
using Eve.Repositories.Context;
using Eve.Repositories.Interfaces.Types;
using Eve.Repositories.Types;
using Eve.Configurations;
using Eve.Repositories.Interfaces.Planets;
using Eve.Repositories.Planets;
using Microsoft.EntityFrameworkCore;
using Eve.Services.Interfaces.Wrappers;
using Eve.Services.Wrappers;
using Eve.Services.Interfaces.EveApi.RefreshTypes;
using Eve.Services.EveApi.RefreshServices;
using Eve.Services.EveApi.EveTypes;
using Eve.Services.Interfaces.EveApi.EveTypes;
using Eve.Services.Interfaces.Orders;
using Eve.Services.Orders;

const string _applicationNameConfigurationService = "EveOnlineMarket";
const string _appConfigEnvironmentVariableName = "AppConfigConnectionString";

var builder = WebApplication.CreateBuilder(args);

builder
    .Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

string appConfigConnectionString =
            // Windows config value
            builder.Configuration[_appConfigEnvironmentVariableName]
            // Linux config value
            ?? builder.Configuration[$"Values:{_appConfigEnvironmentVariableName}"]
            ?? throw new ArgumentNullException(_appConfigEnvironmentVariableName);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddAzureAppConfiguration(appConfigConnectionString)
    .Build();
 
var connectionString = builder.Configuration
    .GetValue(typeof(string), "EveOnlineMarket:ConnectionString")?
    .ToString();
Console.WriteLine($"Connection String: {connectionString}");
if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IAuthenticationService, OAuth2AuthenticationService>();
builder.Services.AddScoped<IUserRepository, PostgresUserRepository>();
builder.Services.AddScoped<IEveApi, EveApiService>();
builder.Services.AddSingleton(builder.Configuration);
builder.Services.AddDbContext<EveDbContext>(opts => 
    opts.UseNpgsql(connectionString, options => options.MigrationsAssembly("Eve.Mvc"))
        .EnableSensitiveDataLogging()
        .LogTo(Console.WriteLine, LogLevel.Information));
builder.Services.AddScoped<IPlanetRepository, PostgresPlanetRepository>();
builder.Services.AddScoped<IHttpClientWrapper, HttpClientWrapper>();
builder.Services.AddScoped<IRefreshTypes, RefreshTypesService>();
builder.Services.AddScoped<IEveTypeService, EveTypeService>();
builder.Services.AddScoped<ITypeRepository, PostgresTypeRepository>();
builder.Services.AddScoped<IOrdersService, OrdersService>();

builder.Services.AddOptions<EveOnlineMarketConfigurationService>()
    .Configure<IConfiguration>((settings, configuration) =>
    {
        configuration.GetSection(_applicationNameConfigurationService).Bind(settings);
    });

builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".EveOnlineMarket.Session";
    options.IdleTimeout = TimeSpan.FromDays(1);
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
