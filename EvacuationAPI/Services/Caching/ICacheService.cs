using EvacuationAPI.DTOs;

namespace EvacuationAPI.Caching;

public interface ICacheService
{
    T? Get<T>(string key);
    
    T? Get<T>();
    
    void Set<T>(string key, T value);
    
    void Set<T>( T value);
    
    void Update<T>(string key, T value);
    
    void Update<T>( T? value);
    
    void Clear();
    
  
    
}