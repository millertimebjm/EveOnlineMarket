
using System.Net;

namespace Eve.Services.Interfaces.Wrappers;

public interface IHttpResponseMessageWrapper
{
    void EnsureSuccessStatusCode();
    IHttpContentWrapper Content { get; }
    HttpStatusCode StatusCode { get; }
}