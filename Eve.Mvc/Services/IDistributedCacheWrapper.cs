namespace Eve.Mvc.Services;

public interface IDistributedCacheWrapper
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value);
}