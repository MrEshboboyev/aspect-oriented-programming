using System;

namespace AspectOrientedProgramming.API.Attributes;

/// <summary>
/// Attribute to mark methods that should invalidate cache entries
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class InvalidateCacheAttribute : Attribute
{
    /// <summary>
    /// The cache key pattern to invalidate
    /// </summary>
    public string? CacheKeyPattern { get; }

    /// <summary>
    /// Whether to invalidate all cache entries
    /// </summary>
    public bool InvalidateAll { get; set; } = false;

    public InvalidateCacheAttribute(string cacheKeyPattern)
    {
        CacheKeyPattern = cacheKeyPattern;
    }

    public InvalidateCacheAttribute()
    {
        InvalidateAll = true;
    }
}