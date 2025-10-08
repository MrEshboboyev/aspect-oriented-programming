using AspectOrientedProgramming.API.Attributes;
using AspectOrientedProgramming.API.Logging;
using Castle.DynamicProxy;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;

namespace AspectOrientedProgramming.API.Aspects;

/// <summary>
/// Interceptor that provides caching functionality for method results with detailed logging
/// </summary>
public class CachingInterceptor(
    IMemoryCache cache,
    ILogger<CachingInterceptor> logger
) : BaseInterceptor
{
    /// <summary>
    /// Determines whether the method should be intercepted for caching
    /// </summary>
    /// <param name="invocation">The method invocation context</param>
    /// <returns>True if the method should be cached, false otherwise</returns>
    public override bool ShouldIntercept(IInvocation invocation)
    {
        // Check if method has Cache attribute
        return invocation.Method.GetCustomAttribute<CacheAttribute>() != null;
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

        // Only cache methods that return a value and have no void return type
        if (invocation.Method.ReturnType == typeof(void))
        {
            invocation.Proceed();
            return;
        }

        var cacheKey = GenerateCacheKey(invocation);
        var methodName = GetMethodName(invocation.Method);
        
        // Try to get result from cache
        if (cache.TryGetValue(cacheKey, out var cachedResult))
        {
            logger.LogInformation(LogMessageTemplates.CacheHit,
                LoggingInterceptor.CorrelationId,
                DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                methodName, 
                cacheKey);
            
            invocation.ReturnValue = cachedResult;
            return;
        }

        logger.LogInformation(LogMessageTemplates.CacheMiss,
            LoggingInterceptor.CorrelationId,
            DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"),
            methodName, 
            cacheKey);
        
        // Execute the method
        invocation.Proceed();

        // Get cache duration from attribute
        var cacheAttribute = invocation.Method.GetCustomAttribute<CacheAttribute>();
        var cacheDuration = TimeSpan.FromMinutes(cacheAttribute?.DurationInMinutes ?? 5);

        // Cache the result
        cache.Set(cacheKey, invocation.ReturnValue, cacheDuration);
        
        logger.LogInformation(LogMessageTemplates.CacheSet,
            LoggingInterceptor.CorrelationId,
            DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"),
            methodName, 
            cacheKey, 
            cacheDuration.TotalMinutes);
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