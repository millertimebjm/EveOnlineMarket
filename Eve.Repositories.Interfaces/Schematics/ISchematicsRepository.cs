using Eve.Models.EveApi;

namespace Eve.Repositories.Interfaces.Schematics;

public interface ISchematicsRepository
{
    Task<Schematic?> Get(int schematicId);

    Task<List<Schematic>> GetAll(List<int> schematicIds);

    Task<Schematic> Upsert(Schematic schematic);
}