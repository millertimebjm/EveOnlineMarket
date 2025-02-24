using System.Text.Json.Serialization;

namespace Eve.Models.EveApi;

public class Planet
{
    [JsonPropertyName("planet_id")]
    public int PlanetId { get; set; }
    public string Name { get; set; } = "";
}