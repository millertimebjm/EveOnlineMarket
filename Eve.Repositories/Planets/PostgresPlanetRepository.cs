using Eve.Models.EveApi;
using Microsoft.EntityFrameworkCore;
using Eve.Repositories.Interfaces.Planets;
using Eve.Repositories.Context;

namespace Eve.Repositories.Planets;

public class PostgresPlanetRepository : IPlanetRepository
{
    private readonly IDbContextFactory<EveDbContext> _dbContextFactory;

    public PostgresPlanetRepository(IDbContextFactory<EveDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<IEnumerable<Planet>> GetAll()
    {
        var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.Planets.ToListAsync();
    }

    public async Task<Planet?> Get(int planetId)
    {
        var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.Planets.SingleOrDefaultAsync(t => t.PlanetId == planetId);
    }

    public async Task<Planet> Upsert(Planet planet)
    {
        var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var existing = await dbContext.Planets
            .SingleOrDefaultAsync(t => t.PlanetId == planet.PlanetId);

        if (existing == null)
        {
            existing = planet;
            await dbContext.Planets.AddAsync(planet);
        }
        else
        {
            var entry = dbContext.Entry(existing);
            var hasChanges = entry.Properties.Any(p => p.IsModified);
            if (hasChanges) await dbContext.SaveChangesAsync();
        }

        await dbContext.SaveChangesAsync();
        return existing;
    }

    public async Task<List<Planet>> GetMany(List<int> planetIds)
    {
        var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.Planets.Where(p => planetIds.Contains(p.PlanetId)).ToListAsync();
    }
}