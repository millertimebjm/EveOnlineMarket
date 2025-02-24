using Eve.Models.EveApi;
using Eve.Models;

namespace Eve.Repositories.Interfaces.Planets;

public interface IPlanetRepository
{
    Task<IEnumerable<Planet>> GetAll();
    Task<Planet?> Get(int planetId);
    Task<Planet> Upsert(Planet planet);
    Task<List<Planet>> GetMany(List<int> planetIds);
}