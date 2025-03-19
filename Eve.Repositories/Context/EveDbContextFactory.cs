using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Eve.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Eve.Configurations.Interfaces;

namespace Eve.Repositories.Context;

public class EveDbContextFactory : IDbContextFactory<EveDbContext>
{
    private readonly DbContextOptions<EveDbContext> _options;
    private readonly IServiceScopeFactory _scopeFactory;

    public EveDbContextFactory(
        DbContextOptions<EveDbContext> options,
        IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        _options = options;
    }

    public EveDbContext CreateDbContext()
    {
        using var scope = _scopeFactory.CreateScope();
        var optionsSnapshot = scope.ServiceProvider
            .GetRequiredService<IOptionsSnapshot<EveOnlineMarketConfigurationService>>();
        return new EveDbContext(_options, optionsSnapshot);
    }
}