using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace AspectOrientedProgramming.API.Aspects;

/// <summary>
/// Registry to track cache keys and their dependencies for proper cache invalidation
/// </summary>
public class CacheKeyRegistry
{
    // Track cache keys by method signature
    private static readonly ConcurrentDictionary<string, string> _methodCacheKeys = new();
    
    // Track which cache keys should be invalidated by which patterns
    private static readonly ConcurrentDictionary<string, HashSet<string>> _invalidationPatterns = new();

    /// <summary>
    /// Registers a cache key for a method
    /// </summary>
    /// <param name="methodSignature">The method signature</param>
    /// <param name="cacheKey">The cache key</param>
    public static void RegisterCacheKey(string methodSignature, string cacheKey)
    {
        _methodCacheKeys.TryAdd(methodSignature, cacheKey);
    }

    /// <summary>
    /// Registers an invalidation pattern for a method
    /// </summary>
    /// <param name="invalidationPattern">The invalidation pattern</param>
    /// <param name="cacheKey">The cache key that should be invalidated</param>
    public static void RegisterInvalidationPattern(string invalidationPattern, string cacheKey)
    {
        _invalidationPatterns.AddOrUpdate(
            invalidationPattern,
            _ => new HashSet<string> { cacheKey },
            (_, existingSet) =>
            {
                existingSet.Add(cacheKey);
                return existingSet;
            });
    }

    /// <summary>
    /// Invalidates cache entries based on a method signature pattern
    /// </summary>
    /// <param name="methodSignaturePattern">The method signature pattern to invalidate</param>
    /// <param name="cache">The memory cache to invalidate entries in</param>
    public static void InvalidateCacheByPattern(string methodSignaturePattern, IMemoryCache cache)
    {
        var keysToInvalidate = new List<string>();
        
        // For simple pattern matching, we'll check if the pattern matches any registered cache keys
        foreach (var kvp in _methodCacheKeys)
        {
            if (PatternMatches(kvp.Key, methodSignaturePattern))
            {
                keysToInvalidate.Add(kvp.Value);
            }
        }
        
        // Remove from cache
        foreach (var key in keysToInvalidate)
        {
            cache.Remove(key);
            // Also remove from registry
            var keyToRemove = _methodCacheKeys.FirstOrDefault(kvp => kvp.Value == key).Key;
            if (keyToRemove != null)
            {
                _methodCacheKeys.TryRemove(keyToRemove, out _);
            }
        }
    }

    /// <summary>
    /// Invalidates all cache entries
    /// </summary>
    /// <param name="cache">The memory cache to invalidate entries in</param>
    public static void InvalidateAll(IMemoryCache cache)
    {
        foreach (var kvp in _methodCacheKeys)
        {
            cache.Remove(kvp.Value);
        }
        
        _methodCacheKeys.Clear();
    }

    /// <summary>
    /// Checks if a method signature matches a pattern
    /// </summary>
    /// <param name="methodSignature">The method signature</param>
    /// <param name="pattern">The pattern to match</param>
    /// <returns>True if the method signature matches the pattern</returns>
    private static bool PatternMatches(string methodSignature, string pattern)
    {
        // Handle wildcard pattern
        if (pattern == "*")
            return true;
            
        // Handle simple contains match
        if (pattern.Contains("*"))
        {
            var regexPattern = Regex.Escape(pattern).Replace("\\*", ".*");
            return Regex.IsMatch(methodSignature, $"^{regexPattern}$");
        }
        
        // Handle exact match with placeholder resolution
        // Replace placeholders like {id} with regex wildcards
        var regexPatternWithPlaceholders = Regex.Escape(pattern)
            .Replace("\\{", "{")
            .Replace("\\}", "}")
            .Replace(@"\{[^\}]*\}", @"[^\)]*"); // Replace {param} with regex pattern
            
        // Check if method signature matches the pattern
        return methodSignature.StartsWith(pattern.Split('(')[0]) || 
               Regex.IsMatch(methodSignature, regexPatternWithPlaceholders);
    }
}