
using System.Net;
using Eve.Services.Interfaces.Wrappers;

namespace Eve.Services.Wrappers;

public class HttpResponseMessageWrapper : IHttpResponseMessageWrapper
{
    private readonly HttpResponseMessage _message;
    public HttpResponseMessageWrapper(HttpResponseMessage message) 
    {
        _message = message;
    }

    public void EnsureSuccessStatusCode()
    {
        _message.EnsureSuccessStatusCode();
    }

    public IHttpContentWrapper Content
    {
        get { return new HttpContentWrapper(_message.Content); }
    }

    public HttpStatusCode StatusCode
    {
        get { return _message.StatusCode; }
    }
}