using Microsoft.Extensions.Caching.Memory;

namespace Othala.Cache;

public interface ICacheService
{
    bool TryGet<T>(string key, string parameter, out T? value);
    Task<T> GetOrCreate<T>(string key, string parameter, Func<Task<T>> itemFactory);
    void Set<T>(string key, string parameter, T value);
    void Remove(string key, params string[] parameters);
    void Clear(string key);
    void Clear();
}

internal class CacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly List<string> _cacheLocks = new ();

    public CacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public bool TryGet<T>(string key, string parameter, out T? value)
    {
        if (!_cacheLocks.Contains($"{key}#{parameter}"))
        {
            value = default;
            return false;
        }
        
        return _memoryCache.TryGetValue($"{key}#{parameter}", out value);
    }

    public async Task<T> GetOrCreate<T>(string key, string parameter, Func<Task<T>> itemFactory)
    {
        T? value;
        
        if (_cacheLocks.Contains($"{key}#{parameter}"))
        {
            if (_memoryCache.TryGetValue($"{key}#{parameter}", out value))
            {
                return value!;
            }
        }

        value = await itemFactory();

        _memoryCache.Set($"{key}#{parameter}", value, TimeSpan.FromMinutes(30));

        if (!_cacheLocks.Contains($"{key}#{parameter}"))
        {
            _cacheLocks.Add($"{key}#{parameter}");
        }

        return value!;
    }

    public void Set<T>(string key, string parameter, T value)
    {
        _memoryCache.Set($"{key}#{parameter}", value, TimeSpan.FromMinutes(30));

        if (!_cacheLocks.Contains($"{key}#{parameter}"))
        {
            _cacheLocks.Add($"{key}#{parameter}");
        }
    }

    public void Remove(string key, params string[] parameters)
    {
        foreach (var parameter in parameters)
        {
            _memoryCache.Remove($"{key}#{parameter}");
            _cacheLocks.Remove($"{key}#{parameter}");
        }
    }

    public void Clear(string key)
    {
            var keysToRemove = _cacheLocks.
                Where(v => v.StartsWith(key))
                .ToList();

            foreach (var keyToRemove in keysToRemove)
            {
                _memoryCache.Remove(keyToRemove);
                _cacheLocks.Remove(keyToRemove);
            }
    }

    public void Clear()
    {
        
        var keysToRemove = _cacheLocks.ToList();

        foreach (var keyToRemove in keysToRemove)
        {
            _memoryCache.Remove(keyToRemove);
            _cacheLocks.Remove(keyToRemove);
        }
    }
}