using AspectOrientedProgramming.API.Attributes;
using AspectOrientedProgramming.API.Logging;
using Castle.DynamicProxy;
using System.Diagnostics;
using System.Reflection;

namespace AspectOrientedProgramming.API.Aspects;

/// <summary>
/// Interceptor that monitors method execution performance with detailed timing information
/// </summary>
public class PerformanceInterceptor(
    ILogger<PerformanceInterceptor> logger
) : BaseInterceptor
{
    private static readonly AsyncLocal<Dictionary<string, Stopwatch>> _stopwatches = new();

    private static Dictionary<string, Stopwatch> Stopwatches
    {
        get => _stopwatches.Value ??= [];
    }

    /// <summary>
    /// Determines whether the method should be intercepted for performance monitoring
    /// </summary>
    /// <param name="invocation">The method invocation context</param>
    /// <returns>True if the method should be monitored, false otherwise</returns>
    public override bool ShouldIntercept(IInvocation invocation)
    {
        // Check if method has Performance attribute
        return invocation.Method.GetCustomAttribute<PerformanceAttribute>() != null;
    }

    /// <summary>
    /// Executed before the method invocation
    /// </summary>
    /// <param name="invocation">The method invocation context</param>
    protected override void OnBefore(IInvocation invocation)
    {
        var methodName = GetMethodName(invocation.Method);
        var stopwatch = Stopwatch.StartNew();
        
        // Store stopwatch for later use
        var contextKey = $"{methodName}_{Guid.NewGuid()}";
        Stopwatches[contextKey] = stopwatch;
        invocation.Method.GetType().GetProperty("PerformanceContextKey")?.SetValue(invocation.Method, contextKey);
        
        logger.LogDebug(LogMessageTemplates.PerformanceMonitoringStarted,
            LoggingInterceptor.CorrelationId,
            DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"),
            methodName);
    }

    /// <summary>
    /// Executed after the method invocation (always executed)
    /// </summary>
    /// <param name="invocation">The method invocation context</param>
    protected override void OnAfter(IInvocation invocation)
    {
        var methodName = GetMethodName(invocation.Method);
        
        // Get execution time
        var contextKey = invocation.Method.GetType().GetProperty("PerformanceContextKey")?.GetValue(invocation.Method)?.ToString();
        var elapsedMs = 0L;
        if (contextKey != null && Stopwatches.TryGetValue(contextKey, out var stopwatch))
        {
            stopwatch.Stop();
            elapsedMs = stopwatch.ElapsedMilliseconds;
            Stopwatches.Remove(contextKey);
        }
        
        // Get warning threshold from attribute
        var performanceAttribute = invocation.Method.GetCustomAttribute<PerformanceAttribute>();
        var warningThreshold = performanceAttribute?.WarningThresholdMs ?? 1000;
        
        // Log performance information
        if (elapsedMs > warningThreshold)
        {
            logger.LogWarning(LogMessageTemplates.PerformanceThresholdExceeded,
                LoggingInterceptor.CorrelationId,
                DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                methodName, 
                elapsedMs, 
                warningThreshold);
        }
        else
        {
            logger.LogInformation(LogMessageTemplates.PerformanceCompleted,
                LoggingInterceptor.CorrelationId,
                DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                methodName, 
                elapsedMs);
        }
    }
}