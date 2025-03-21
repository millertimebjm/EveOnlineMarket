using Eve.Models.EveApi;
using Microsoft.EntityFrameworkCore;
using Eve.Repositories.Interfaces.Types;
using Eve.Repositories.Context;
using Eve.Models.EveTypes;

namespace Eve.Repositories.Types;

public class PostgresTypeRepository(IDbContextFactory<EveDbContext> _dbContextFactory) : ITypeRepository
{
    public async Task<IEnumerable<EveType>> GetAll()
    {
        var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.Types.ToListAsync();
    }

    public async Task<EveType?> Get(int typeId)
    {
        var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.Types.SingleOrDefaultAsync(t => t.TypeId == typeId);
    }

    public async Task<EveType> Upsert(EveType type)
    {
        var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var existingType = await dbContext.Types
            .SingleOrDefaultAsync(t => t.TypeId == type.TypeId);

        if (existingType == null)
        {
            await dbContext.Types.AddAsync(type);
        }
        else
        {
            var entry = dbContext.Entry(existingType);
            var hasChanges = entry.Properties.Any(p => p.IsModified);
            if (hasChanges) await dbContext.SaveChangesAsync();
        }

        await dbContext.SaveChangesAsync();
        return type;
    }

    public async Task<List<EveType>> GetMarketableTypes()
    {
        var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.Types.Where(t => t.MarketGroupId > 0).ToListAsync();
    }

    public async Task<List<EveType>> Search(EveTypeSearchFilterModel model)
    {
        var dbContext = await _dbContextFactory.CreateDbContextAsync();
        IQueryable<EveType> query = dbContext.Types;
        
        query = query.Where(q => !model.IsMarketableType || q.MarketGroupId > 0);

        if (model.TypeIds.Any() || model.SchematicsIds.Any())
        {
            query = query.Where(t => model.TypeIds.Contains(t.TypeId) || (t.SchematicId != null && model.SchematicsIds.Contains(t.SchematicId.Value)));
        }

        if (!string.IsNullOrWhiteSpace(model.Keyword))
        {
            query = query.Where(q => q.Name.ToLower().Contains(model.Keyword.ToLower()));
        }

        query = query.OrderBy(q => q.Name);
        if (model.Skip > 0) query = query.Skip(model.Skip);
        query = query.Take(model.Take);

        return await query.ToListAsync();
    }
}