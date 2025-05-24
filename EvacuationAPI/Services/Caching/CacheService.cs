using System.Text.Json;
using EvacuationAPI.DTOs;
using Microsoft.Extensions.Caching.Distributed;
using Volo.Abp.Caching;

namespace EvacuationAPI.Caching;

public class CacheService : ICacheService

{
    public static readonly string StatusKey = "status";
    private readonly IDistributedCache? _cache;
    private readonly ILogger<CacheService> _logger;

    public CacheService(IDistributedCache? cache, ILogger<CacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }
    
    public T? Get<T>(string key)
    {
        var data = _cache?.GetString(key);
        
        if (data == null)
        {
            _logger.LogDebug("Cache miss for key: {Key}", key);
            return default;
        }
        _logger.LogDebug("Cache hit for key: {Key}", key);
        
        _logger.LogDebug("Cache Data:" + data);
        return JsonSerializer.Deserialize<T>(data);
    }

    public T? Get<T>()
    {
        return Get<T>(StatusKey);
    }

    public void Set<T>(string key, T value)
    {
       try 
       {
           var options = new DistributedCacheEntryOptions();
           options.SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
           _cache?.SetString(key, JsonSerializer.Serialize(value), options);
           _logger.LogInformation("Cache updated for key: {Key}, value: {@Value}, expiration: {Expiration}", 
               key, value, DateTime.UtcNow.AddMinutes(10));
       }
       catch (Exception ex)
       {
           _logger.LogError(ex, "Failed to set cache for key: {Key}", key);
       }
    }

    public void Set<T>(T value)
    {
        Set<T>(StatusKey, value);
    }

    public void Update<T>(string key, T value)
    {
        if (_cache != null)
        {
            try
            {
                _cache.Remove(key);
                _logger.LogInformation("Cache removed for key: {Key}", key);
                Set(key, value);
                _logger.LogDebug("Cache successfully updated for key: {Key} with value: {@Value}", key, value);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to update cache for key: {Key}", key);
                throw;
            }
        }
    }

    public void Update<T>(T value)
    {
  
        Update<T>(StatusKey, value);
    }

    public void Clear()
    {
        try
        {
            _cache.Remove(StatusKey);
            _logger.LogInformation("Cache cleared for key: {Key}", StatusKey);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to clear cache for key: {Key}", StatusKey);
            throw;
        }
    }
}