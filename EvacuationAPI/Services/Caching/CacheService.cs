using System.Text.Json;
using EvacuationAPI.DTOs;
using Microsoft.Extensions.Caching.Distributed;
using Volo.Abp.Caching;

namespace EvacuationAPI.Caching;

public class CacheService : ICacheService

{
    public static readonly string StatusKey = "status";
    private readonly IDistributedCache? _cache;

    public CacheService(IDistributedCache? cache)
    {
        _cache = cache;
    }
    
    public T? Get<T>(string key)
    {
        var data = _cache?.GetString(key);
        if (data == null) return default;
        return JsonSerializer.Deserialize<T>(data);
    }

    public T? Get<T>()
    {
        return Get<T>(StatusKey);
    }

    public void Set<T>(string key, T value)
    {
       var options = new DistributedCacheEntryOptions();
       options.SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
       _cache?.SetString(key, JsonSerializer.Serialize(value), options);
    }

    public void Set<T>(T value)
    {
        Set<T>(StatusKey, value);
    }

    public void Update<T>(string key, T value)
    {
        if (_cache != null)
        {
            _cache.Remove(key);
            Set(key, value);
        }
        
    }

    public void Update<T>(T value)
    {
        Update<T>(StatusKey, value);
    }
}