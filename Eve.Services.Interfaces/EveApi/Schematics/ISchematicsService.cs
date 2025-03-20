using Eve.Models.EveApi;

namespace Eve.Services.Interfaces.EveApi.Schematics;

public interface ISchematicsService
{
    Task<Schematic> Get(int schematicId, string accessToken);
    Task<List<Schematic>> GetAll(List<int> schematicIds, string accessToken);
    Task<Schematic> Upsert(Schematic schematic, string accessToken);
}