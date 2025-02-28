
using System.Net.Http.Json;
using Eve.Services.Interfaces.Wrappers;

namespace Eve.Services.Wrappers;

public class HttpContentWrapper : IHttpContentWrapper
{
    private readonly HttpContent _content;
    public HttpContentWrapper(HttpContent content) 
    {
        _content = content;
    }

    public async Task<string> ReadAsStringAsync()
    {
        return await _content.ReadAsStringAsync();
    }

    public async Task<T?> ReadFromJsonAsync<T>()
    {
        return await _content.ReadFromJsonAsync<T>();
    }
}