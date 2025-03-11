using Eve.Models.EveApi;
using Microsoft.EntityFrameworkCore;
using Eve.Repositories.Interfaces.Types;
using Eve.Repositories.Context;
using Eve.Models.EveTypes;

namespace Eve.Repositories.Types;

public class PostgresTypeRepository : ITypeRepository
{
    private readonly EveDbContext _dbContext;

    public PostgresTypeRepository(EveDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<EveType>> GetAll()
    {
        return await _dbContext.Types.ToListAsync();
    }

    public async Task<EveType?> Get(int typeId)
    {
        return await _dbContext.Types.SingleOrDefaultAsync(t => t.TypeId == typeId);
    }

    public async Task<EveType> Upsert(EveType type)
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

    public async Task<List<EveType>> GetMarketableTypes()
    {
        return await _dbContext.Types.Where(t => t.MarketGroupId > 0).ToListAsync();
    }

    public async Task<List<EveType>> Search(EveTypeSearchFilterModel model)
    {
        IQueryable<EveType> query = _dbContext.Types;

        query = query.Where(q => q.MarketGroupId > 0);

        if (model.Ids != null && model.Ids.Any())
        {
            _dbContext.Types.Where(t => model.Ids.Contains(t.TypeId));
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