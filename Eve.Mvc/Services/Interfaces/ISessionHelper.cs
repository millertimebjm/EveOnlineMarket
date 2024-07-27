
namespace Eve.Mvc.Services.Interfaces;

public interface ISessionHelper
{
    void Add(string key, object value);
    object Get(string key);
}