
using System.Text;
using System.Text.Json;
using Eve.Mvc.Models;
using Eve.Mvc.Services.Interfaces;

namespace Eve.Mvc.Services;

public class OAuth2AuthenticationService : IAuthenticationService
{
    private const string _eveOnlineApiOauthUrl = "https://login.eveonline.com/v2/oauth/token";
    private readonly IHttpClientFactory _httpClientFactory;
    public OAuth2AuthenticationService(
        IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    public async Task<AuthenticationResponseModel> GetAccessToken(
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

    public async Task<AuthenticationResponseModel> RefreshAccessToken(
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

    private async Task<AuthenticationResponseModel> GetTokenPrivate(
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
        var httpClient = _httpClientFactory.CreateClient();
        HttpResponseMessage response = await httpClient.SendAsync(message);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<AuthenticationResponseModel>();
    }
}