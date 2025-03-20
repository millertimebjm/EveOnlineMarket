using Eve.Models.EveApi;
using Microsoft.EntityFrameworkCore;
using Eve.Repositories.Interfaces.Schematics;
using Eve.Repositories.Context;

namespace Eve.Repositories.Schematics;

public class PostgresSchematicsRepository(IDbContextFactory<EveDbContext> _dbContextFactory) : ISchematicsRepository
{
    private readonly IDbContextFactory<EveDbContext> _dbContextFactory = _dbContextFactory;

    public async Task<Schematic?> Get(int schematicId)
    {
        var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return dbContext.Schematics.SingleOrDefault(s => s.SchematicId == schematicId);
    }

    public async Task<List<Schematic>> GetAll(List<int> schematicIds)
    {
        var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return dbContext.Schematics.Where(s => schematicIds.Contains(s.SchematicId)).ToList();
    }

    public async Task<Schematic> Upsert(Schematic schematic)
    {
        var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var existingSchematic = await dbContext.Schematics
            .SingleOrDefaultAsync(s => s.SchematicId == schematic.SchematicId);

        if (existingSchematic == null)
        {
            await dbContext.Schematics.AddAsync(schematic);
        }
        else
        {
            var entry = dbContext.Entry(existingSchematic);
            var hasChanges = entry.Properties.Any(p => p.IsModified);
            if (hasChanges) await dbContext.SaveChangesAsync();
        }

        await dbContext.SaveChangesAsync();
        return schematic;
    }
}