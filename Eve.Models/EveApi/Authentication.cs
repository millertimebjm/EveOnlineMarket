using System.Text.Json.Serialization;

namespace Eve.Models.EveApi;

public class Authentication
{
    [JsonPropertyName("access_token")]
    public string AccessToken {get; set;} = "";
    [JsonPropertyName("refresh_token")]
    public string RefreshToken {get; set;} = "";
    public DateTime IssuedDate {get; set;} = DateTime.UtcNow;
    [JsonPropertyName("expires_in")]
    public int ExpiresIn {get; set;}
}