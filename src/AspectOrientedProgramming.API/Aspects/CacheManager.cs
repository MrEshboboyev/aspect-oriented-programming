using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace AspectOrientedProgramming.API.Aspects;

/// <summary>
/// Simple cache manager to handle cache invalidation
/// </summary>
public static class CacheManager
{
    private static IMemoryCache? _cache;
    private static readonly ConcurrentDictionary<string, object?> _cacheEntries = new();

    /// <summary>
    /// Initialize the cache manager with a memory cache instance
    /// </summary>
    /// <param name="cache">The memory cache instance</param>
    public static void Initialize(IMemoryCache cache)
    {
        _cache = cache;
    }

    /// <summary>
    /// Register a cache entry
    /// </summary>
    /// <param name="key">The cache key</param>
    /// <param name="value">The cached value</param>
    public static void RegisterCacheEntry(string key, object? value)
    {
        _cacheEntries.TryAdd(key, value);
    }

    /// <summary>
    /// Invalidate a specific cache entry
    /// </summary>
    /// <param name="key">The cache key to invalidate</param>
    public static void InvalidateCacheEntry(string key)
    {
        if (_cache != null)
        {
            _cache.Remove(key);
            _cacheEntries.TryRemove(key, out _);
        }
    }

    /// <summary>
    /// Invalidate cache entries by pattern
    /// </summary>
    /// <param name="pattern">The pattern to match cache keys</param>
    public static void InvalidateCacheByPattern(string pattern)
    {
        if (_cache == null) return;

        var keysToRemove = new List<string>();
        foreach (var kvp in _cacheEntries)
        {
            // Improved pattern matching logic
            if (PatternMatches(kvp.Key, pattern))
            {
                keysToRemove.Add(kvp.Key);
            }
        }

        foreach (var key in keysToRemove)
        {
            _cache.Remove(key);
            _cacheEntries.TryRemove(key, out _);
        }
    }

    /// <summary>
    /// Check if a cache key matches a pattern
    /// </summary>
    /// <param name="key">The cache key</param>
    /// <param name="pattern">The pattern to match</param>
    /// <returns>True if the key matches the pattern</returns>
    private static bool PatternMatches(string key, string pattern)
    {
        // Handle exact matches
        if (key.Equals(pattern, StringComparison.OrdinalIgnoreCase))
            return true;

        // Handle patterns with wildcards
        if (pattern.Contains("*"))
        {
            // Convert pattern to regex
            var regexPattern = Regex.Escape(pattern).Replace("\\*", ".*");
            var result = Regex.IsMatch(key, $"^{regexPattern}$", RegexOptions.IgnoreCase);
            System.Diagnostics.Debug.WriteLine($"Wildcard pattern matching: '{key}' vs '{pattern}' -> {result}");
            return result;
        }

        // Handle patterns with parameter placeholders like {id}
        if (pattern.Contains("{") && pattern.Contains("}"))
        {
            // Convert the pattern to a regex that matches the method signature
            // For example, "TestCacheService.GetCachedData({id})" should match "TestCacheService.GetCachedData(1)"
            var regexPattern = Regex.Escape(pattern);
            // Replace {param} with a pattern that matches anything until the closing parenthesis
            regexPattern = Regex.Replace(regexPattern, @"\\\{[^}]+\\\}", @"[^)]*");
            regexPattern = $"^{regexPattern}$";
            var result = Regex.IsMatch(key, regexPattern, RegexOptions.IgnoreCase);
            // For debugging - in a real implementation you might want to use a proper logging framework
            System.Diagnostics.Debug.WriteLine($"Parameter pattern matching: '{key}' vs '{pattern}' -> {result}");
            return result;
        }

        // Handle prefix matching for method names
        if (key.StartsWith(pattern.Split('(')[0]))
        {
            System.Diagnostics.Debug.WriteLine($"Prefix pattern matching: '{key}' vs '{pattern}' -> true");
            return true;
        }

        System.Diagnostics.Debug.WriteLine($"No pattern match: '{key}' vs '{pattern}' -> false");
        return false;
    }

    /// <summary>
    /// Invalidate all cache entries
    /// </summary>
    public static void InvalidateAll()
    {
        if (_cache == null) return;

        foreach (var key in _cacheEntries.Keys)
        {
            _cache.Remove(key);
        }
        _cacheEntries.Clear();
    }
}