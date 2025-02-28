
using Eve.Services.Interfaces.Wrappers;

namespace Eve.Services.Wrappers;

public class HttpClientWrapper : IHttpClientWrapper
{
    private readonly IHttpClientFactory _httpClientFactory;
    private HttpClient? _httpClient;
    private HttpClient HttpClient
    {
        get
        {
            if (_httpClient == null)
            {
                _httpClient = _httpClientFactory.CreateClient();
            }
            return _httpClient;
        }
    }
    public HttpClientWrapper(IHttpClientFactory httpClientFactory) 
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IHttpResponseMessageWrapper> GetAsync(Uri uri)
    {
        
        return new HttpResponseMessageWrapper(await HttpClient.GetAsync(uri));
    }

    public void AddDefaultRequestHeaders(string name, string value)
    {
        //DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
        HttpClient.DefaultRequestHeaders.Add(name, value);
    }
    
}