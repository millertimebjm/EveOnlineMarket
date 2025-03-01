using System.Net.Http.Json;
using System.Text;
using Eve.Models.EveApi;
using Eve.Services.Interfaces.Authentications;
using Eve.Services.Interfaces.Wrappers;

namespace Eve.Services.Authentications;

public class OAuth2AuthenticationService : IAuthenticationService
{
    private const string _eveOnlineApiOauthUrl = "https://login.eveonline.com/v2/oauth/token";
    private readonly IHttpClientWrapper _httpClientWrapper;
    public OAuth2AuthenticationService(
        IHttpClientWrapper httpClientWrapper)
    {
        _httpClientWrapper = httpClientWrapper;
    }
    public async Task<Authentication> GetAccessToken(
        string authorizationToken, 
        string clientId,
        string clientSecret)
    {
        var bodyContent = new StringContent(
            $"grant_type=authorization_code&code={authorizationToken}",
            Encoding.UTF8, // Specify encoding
            "application/x-www-form-urlencoded" // Add Content-Type here
        );
        
        return await GetTokenPrivate(
            bodyContent,
            clientId,
            clientSecret);
    }

    private static string Base64Encode(string plainText) 
    {
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        return System.Convert.ToBase64String(plainTextBytes);
    }

    public async Task<Authentication> RefreshAccessToken(
        string clientId,
        string clientSecret,
        string refreshToken)
    {
        var bodyContent = new StringContent(
            $"grant_type=refresh_token&refresh_token={refreshToken}",
            Encoding.UTF8, // Specify encoding
            "application/x-www-form-urlencoded" // Add Content-Type here
        );
        return await GetTokenPrivate(
            bodyContent,
            clientId,
            clientSecret);
    }

    private async Task<Authentication> GetTokenPrivate(
        HttpContent bodyContent,
        string clientId,
        string clientSecret)
    {
        var message = new HttpRequestMessage(
            HttpMethod.Post,
            new Uri(_eveOnlineApiOauthUrl));
        message.Content = bodyContent;
        var base64AuthString = Base64Encode($"{clientId}:{clientSecret}");
        message.Headers.Add("Authorization",$"Basic {base64AuthString}");
        message.Headers.Add("Host", "login.eveonline.com");
        var response = await _httpClientWrapper.SendAsync(message);
        response.EnsureSuccessStatusCode();
        var authModel = await response.Content.ReadFromJsonAsync<Authentication>();
        if (authModel == null) throw new Exception("Authentication Response returned as null");
        return authModel;
    }
}