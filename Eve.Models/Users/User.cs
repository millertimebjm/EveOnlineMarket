namespace Eve.Models.Users;

public class User
{
    public long UserId { get; set;}
    public string AuthorizationCode {get; set;} = string.Empty;
    public string AccessToken {get; set;} = string.Empty;
    public DateTime TokenGrantedDateTime {get; set;} = DateTime.MinValue;
    public string BearerToken { get; set; } = string.Empty;
    public string ClientId {get; set;} = string.Empty;
    public string ClientSecret {get; set;} = string.Empty;
    public DateTime TokenExpirationDate { get; set; }
}