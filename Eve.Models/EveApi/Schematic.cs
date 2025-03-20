using System.Text.Json.Serialization;

namespace Eve.Models.EveApi;

public class Schematic
{
    [JsonPropertyName("schematic_id")]
    public int SchematicId { get; set; } = 0;
    [JsonPropertyName("schematic_name")]
    public string SchematicName { get; set; } = "";
    [JsonPropertyName("cycle_time")]
    public int CycleTimeInSeconds { get; set; }
}