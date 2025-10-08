using AspectOrientedProgramming.API.Attributes;
using AspectOrientedProgramming.API.Logging;
using Castle.DynamicProxy;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AspectOrientedProgramming.API.Aspects;

/// <summary>
/// Interceptor that provides caching functionality for method results with detailed logging
/// </summary>
public class CachingInterceptor : BaseInterceptor
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<CachingInterceptor> _logger;

    /// <summary>
    /// Initializes the CachingInterceptor with required dependencies
    /// </summary>
    public CachingInterceptor(IMemoryCache cache, ILogger<CachingInterceptor> logger)
    {
        _cache = cache;
        _logger = logger;
        CacheManager.Initialize(cache);
    }

    /// <summary>
    /// Determines whether the method should be intercepted for caching
    /// </summary>
    /// <param name="invocation">The method invocation context</param>
    /// <returns>True if the method should be cached, false otherwise</returns>
    public override bool ShouldIntercept(IInvocation invocation)
    {
        // Check if method has Cache attribute or InvalidateCache attribute
        return invocation.Method.GetCustomAttribute<CacheAttribute>() != null ||
               invocation.Method.GetCustomAttribute<InvalidateCacheAttribute>() != null;
    }

    /// <summary>
    /// Main interception method that implements caching logic
    /// </summary>
    /// <param name="invocation">The method invocation context</param>
    public override void Intercept(IInvocation invocation)
    {
        // Check if the method should be intercepted
        if (!ShouldIntercept(invocation))
        {
            invocation.Proceed();
            return;
        }

        // Handle cache invalidation first
        var invalidateAttributes = invocation.Method.GetCustomAttributes<InvalidateCacheAttribute>();
        foreach (var attr in invalidateAttributes)
        {
            InvalidateCache(invocation, attr);
        }

        // Check if method has Cache attribute
        var cacheAttribute = invocation.Method.GetCustomAttribute<CacheAttribute>();
        if (cacheAttribute != null)
        {
            HandleCaching(invocation, cacheAttribute);
        }
        else
        {
            // Just proceed with the method execution if only invalidation was needed
            invocation.Proceed();
        }
    }

    /// <summary>
    /// Handles caching logic for methods with Cache attribute
    /// </summary>
    /// <param name="invocation">The method invocation context</param>
    /// <param name="cacheAttribute">The cache attribute</param>
    private void HandleCaching(IInvocation invocation, CacheAttribute cacheAttribute)
    {
        // Only cache methods that return a value and have no void return type
        if (invocation.Method.ReturnType == typeof(void))
        {
            invocation.Proceed();
            return;
        }

        var cacheKey = GenerateCacheKey(invocation);
        var methodName = GetMethodName(invocation.Method);
        
        // Log the cache key for debugging
        _logger.LogInformation("Generated cache key: {CacheKey}", cacheKey);
        
        // Try to get result from cache
        if (_cache.TryGetValue(cacheKey, out var cachedResult))
        {
            _logger.LogInformation(LogMessageTemplates.CacheHit,
                LoggingInterceptor.CorrelationId,
                DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                methodName, 
                cacheKey);
            
            invocation.ReturnValue = cachedResult;
            return;
        }

        _logger.LogInformation(LogMessageTemplates.CacheMiss,
            LoggingInterceptor.CorrelationId,
            DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"),
            methodName, 
            cacheKey);
        
        // Execute the method
        invocation.Proceed();

        // Get cache duration from attribute
        var cacheDuration = TimeSpan.FromMinutes(cacheAttribute.DurationInMinutes);

        // Cache the result
        _cache.Set(cacheKey, invocation.ReturnValue, cacheDuration);
        
        // Register the cache entry with CacheManager for invalidation purposes
        CacheManager.RegisterCacheEntry(cacheKey, invocation.ReturnValue);
        
        _logger.LogInformation(LogMessageTemplates.CacheSet,
            LoggingInterceptor.CorrelationId,
            DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"),
            methodName, 
            cacheKey, 
            cacheDuration.TotalMinutes);
    }

    /// <summary>
    /// Invalidates cache entries based on the invalidate cache attribute
    /// </summary>
    /// <param name="invocation">The method invocation context</param>
    /// <param name="attribute">The invalidate cache attribute</param>
    private void InvalidateCache(IInvocation invocation, InvalidateCacheAttribute attribute)
    {
        if (attribute.InvalidateAll)
        {
            // Invalidate all cache entries using CacheManager
            CacheManager.InvalidateAll();
            _logger.LogInformation("Cache invalidation for all entries completed");
        }
        else if (attribute.CacheKeyPattern != null)
        {
            // Resolve the cache key pattern with actual method arguments
            var resolvedPattern = ResolveCacheKeyPattern(attribute.CacheKeyPattern, invocation);
            
            // Log the pattern for debugging
            _logger.LogInformation("Attempting to invalidate cache with pattern: {CacheKeyPattern}", resolvedPattern);
            
            // Invalidate cache entries by pattern using CacheManager
            CacheManager.InvalidateCacheByPattern(resolvedPattern);
            
            _logger.LogInformation("Cache invalidated for pattern: {CacheKeyPattern}", resolvedPattern);
        }
    }

    /// <summary>
    /// Resolves a cache key pattern with actual method arguments
    /// </summary>
    /// <param name="pattern">The cache key pattern</param>
    /// <param name="invocation">The method invocation context</param>
    /// <returns>The resolved cache key pattern</returns>
    private string ResolveCacheKeyPattern(string pattern, IInvocation invocation)
    {
        var resolvedPattern = pattern;
        
        // Replace method argument placeholders like {id}, {0}, {1}, etc.
        for (int i = 0; i < invocation.Arguments.Length; i++)
        {
            var argValue = invocation.Arguments[i]?.ToString() ?? "null";
            resolvedPattern = resolvedPattern.Replace($"{{{i}}}", argValue);
            
            // Also try to replace named parameters if we can extract them
            var parameters = invocation.Method.GetParameters();
            if (i < parameters.Length)
            {
                resolvedPattern = resolvedPattern.Replace($"{{{parameters[i].Name}}}", argValue);
            }
        }
        
        return resolvedPattern;
    }

    /// <summary>
    /// Generates a cache key based on method name and arguments
    /// </summary>
    /// <param name="invocation">The method invocation context</param>
    /// <returns>A unique cache key</returns>
    private string GenerateCacheKey(IInvocation invocation)
    {
        var key = $"{GetMethodName(invocation.Method)}({string.Join(",", invocation.Arguments.Select(a => a?.ToString() ?? "null"))})";
        return key;
    }
}