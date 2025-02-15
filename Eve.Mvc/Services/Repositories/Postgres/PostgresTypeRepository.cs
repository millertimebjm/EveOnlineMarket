using Eve.Mvc.Models;
using Eve.Mvc.Services.Repositories;
using EveOnlineMarket.Eve.Mvc.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Eve.Mvc.Services.Memory;

public class PostgresTypeRepository : ITypeRepository
{
    private readonly EveDbContext _dbContext;

    public PostgresTypeRepository(EveDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<EveUniverseType>> GetAll()
    {
        return await _dbContext.Types.ToListAsync();
    }

    public async Task<EveUniverseType?> Get(int typeId)
    {
        return await _dbContext.Types.SingleOrDefaultAsync(t => t.TypeId == typeId);
    }

    public async Task<EveUniverseType> Upsert(EveUniverseType type)
    {
        var existingType = await _dbContext.Types
            .SingleOrDefaultAsync(t => t.TypeId == type.TypeId);

        if (existingType == null)
        {
            await _dbContext.Types.AddAsync(type);
        }
        else
        {
            _dbContext.Entry(existingType).CurrentValues.SetValues(type);
        }

        await _dbContext.SaveChangesAsync();
        return type;
    }
}