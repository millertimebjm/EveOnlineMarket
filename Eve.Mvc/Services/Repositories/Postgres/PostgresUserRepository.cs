
using System.Collections.Concurrent;
using Eve.Mvc.Models;
using EveOnlineMarket.Eve.Mvc.Services;
using Microsoft.Extensions.Options;

namespace Eve.Mvc.Services.Memory;

public class PostgresUserRepository : IUserRepository
{
    private readonly EveOnlineMarketConfigurationService _configuration;

    public PostgresUserRepository(IOptionsSnapshot<EveOnlineMarketConfigurationService> options)
    {
        _configuration = options.Value;
    }

    public async Task<IEnumerable<User>> GetAll()
    {
        var values = _items.Values.ToList();
        return await Task.FromResult(values);
    }

    public async Task<User?> Get(long userId)
    {
        var value = _items.GetValueOrDefault(userId);
        return await Task.FromResult(value);
    }

    public async Task<User> Upsert(User user)
    {
        var value = _items.AddOrUpdate(user.UserId, user, (id, oldUser) => user);
        return await Task.FromResult(value);
    }
}