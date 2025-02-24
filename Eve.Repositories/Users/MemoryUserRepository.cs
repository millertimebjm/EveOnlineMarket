using System.Collections.Concurrent;
using Eve.Models.Users;
using Eve.Repositories.Interfaces.Users;

namespace Eve.Repositories.Users;

public class MemoryUserRepository : IUserRepository
{
    private readonly ConcurrentDictionary<long, User> _items;
    public MemoryUserRepository()
    {
        _items = new ConcurrentDictionary<long, User>();
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