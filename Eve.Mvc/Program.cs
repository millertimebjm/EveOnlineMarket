using Eve.Mvc.Services;
using Eve.Mvc.Services.Memory;
using Eve.Mvc.Services.Interfaces;

const string _applicationNameConfigurationService = "EveOnlineMarket";
const string _appConfigEnvironmentVariableName = "AppConfigConnectionString";

var builder = WebApplication.CreateBuilder(args);

builder
    .Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IAuthenticationService, OAuth2AuthenticationService>();
builder.Services.AddSingleton<IUserRepository, MemoryUserRepository>();
builder.Services.AddScoped<IEveApi, EveApiService>();
builder.Services.AddSingleton(builder.Configuration);

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
