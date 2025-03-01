
namespace Eve.Services.Interfaces.Wrappers;

public interface IHttpClientWrapper
{
    public Task<IHttpResponseMessageWrapper> GetAsync(Uri uri);
    public void AddDefaultRequestHeaders(string name, string value);
    public Task<IHttpResponseMessageWrapper> SendAsync(HttpRequestMessage message);
}