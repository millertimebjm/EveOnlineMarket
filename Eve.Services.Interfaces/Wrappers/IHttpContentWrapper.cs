
namespace Eve.Services.Interfaces.Wrappers;

public interface IHttpContentWrapper
{
    Task<string> ReadAsStringAsync();
    Task<T?> ReadFromJsonAsync<T>();
}