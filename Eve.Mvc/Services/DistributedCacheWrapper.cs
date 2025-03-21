using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace Eve.Mvc.Services;

public class DistributedCacheWrapper(IDistributedCache _distributedCache) : IDistributedCacheWrapper
{
    public async Task<T?> GetAsync<T>(string key)
    {
        
        var valueString = await _distributedCache.GetStringAsync(key);
        if (string.IsNullOrWhiteSpace(valueString)) 
        {
            return default;
        }
        return JsonSerializer.Deserialize<T>(valueString);
    }
    public async Task SetAsync<T>(string key, T value)
    {
        if (value == null) await _distributedCache.SetStringAsync(key, "");
        await _distributedCache.SetStringAsync(key, JsonSerializer.Serialize(value));
    }
}