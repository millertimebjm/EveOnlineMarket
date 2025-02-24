
namespace Eve.Services.Interfaces.Session;

public interface ISessionHelper
{
    void Add(string key, object value);
    object Get(string key);
}