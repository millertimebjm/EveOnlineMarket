using Eve.Mvc.Models;
using Eve.Mvc.Services.Repositories;
using Microsoft.EntityFrameworkCore;

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
            var entry = _dbContext.Entry(existingType);
            var hasChanges = entry.Properties.Any(p => p.IsModified);
            if (hasChanges) await _dbContext.SaveChangesAsync();
        }

        await _dbContext.SaveChangesAsync();
        return type;
    }

    public async Task<List<EveUniverseType>> GetMarketableTypes()
    {
        return await _dbContext.Types.Where(t => t.MarketGroupId > 0).ToListAsync();
    }

    public async Task<List<EveUniverseType>> Search(EveUniverseTypeSearchFilterModel searchFilterModel)
    {
        IQueryable<EveUniverseType> query = _dbContext.Types;

        query = query.Where(q => q.MarketGroupId > 0);

        if (!string.IsNullOrWhiteSpace(searchFilterModel.Keyword))
        {
            query = query.Where(q => q.Name.ToLower().Contains(searchFilterModel.Keyword.ToLower()));
        }

        query = query.OrderBy(q => q.Name);
        if (searchFilterModel.Skip > 0) query = query.Skip(searchFilterModel.Skip);
        query = query.Take(searchFilterModel.Take);

        return await query.ToListAsync();
    }

}