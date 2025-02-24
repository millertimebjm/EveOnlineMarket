using Eve.Models.EveApi;
using Eve.Models.Users;
using Eve.Models;
using Microsoft.EntityFrameworkCore;
using Eve.Repositories.Interfaces.Planets;
using Eve.Repositories.Context;

namespace Eve.Repositories.Planets;

public class PostgresPlanetRepository : IPlanetRepository
{
    private readonly EveDbContext _dbContext;

    public PostgresPlanetRepository(EveDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Planet>> GetAll()
    {
        return await _dbContext.Planets.ToListAsync();
    }

    public async Task<Planet?> Get(int planetId)
    {
        return await _dbContext.Planets.SingleOrDefaultAsync(t => t.PlanetId == planetId);
    }

    public async Task<Planet> Upsert(Planet planet)
    {
        var existing = await _dbContext.Planets
            .SingleOrDefaultAsync(t => t.PlanetId == planet.PlanetId);

        if (existing == null)
        {
            existing = planet;
            await _dbContext.Planets.AddAsync(planet);
        }
        else
        {
            var entry = _dbContext.Entry(existing);
            var hasChanges = entry.Properties.Any(p => p.IsModified);
            if (hasChanges) await _dbContext.SaveChangesAsync();
        }

        await _dbContext.SaveChangesAsync();
        return existing;
    }

    public async Task<List<Planet>> GetMany(List<int> planetIds)
    {
        return await _dbContext.Planets.Where(p => planetIds.Contains(p.PlanetId)).ToListAsync();
    }
}