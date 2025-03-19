using Eve.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Eve.Repositories.Interfaces.Users;
using Eve.Repositories.Context;

namespace Eve.Repositories.Users;

public class PostgresUserRepository : IUserRepository
{
    private readonly IDbContextFactory<EveDbContext> _dbContextFactory;

    public PostgresUserRepository(IDbContextFactory<EveDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<IEnumerable<User>> GetAll()
    {
        var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.Users.ToListAsync();
    }

    public async Task<User?> Get(long userId)
    {
        var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.Users.SingleOrDefaultAsync(u => u.UserId == userId);
    }
    

    public async Task<User> Upsert(User user)
    {
        var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var existingUser = await dbContext.Users
            .SingleOrDefaultAsync(u => u.UserId == user.UserId);

        if (existingUser == null)
        {
            await dbContext.Users.AddAsync(user);
        }
        else
        {
            dbContext.Entry(existingUser).CurrentValues.SetValues(user);
        }

        await dbContext.SaveChangesAsync();
        return user;
    }
}