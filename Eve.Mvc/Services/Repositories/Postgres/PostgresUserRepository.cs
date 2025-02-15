using Eve.Mvc.Models;
using Eve.Mvc.Services.Repositories;
using EveOnlineMarket.Eve.Mvc.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Eve.Mvc.Services.Memory;

public class PostgresUserRepository : IUserRepository
{
    private readonly EveDbContext _dbContext;

    public PostgresUserRepository(EveDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<User>> GetAll()
    {
        return await _dbContext.Users.ToListAsync();
    }

    public async Task<User?> Get(long userId)
    {
        return await _dbContext.Users.SingleOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task<User> Upsert(User user)
    {
        var existingUser = await _dbContext.Users
            .SingleOrDefaultAsync(u => u.UserId == user.UserId);

        if (existingUser == null)
        {
            await _dbContext.Users.AddAsync(user);
        }
        else
        {
            _dbContext.Entry(existingUser).CurrentValues.SetValues(user);
        }

        await _dbContext.SaveChangesAsync();
        return user;
    }
}