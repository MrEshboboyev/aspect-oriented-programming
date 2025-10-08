using AspectOrientedProgramming.API.Attributes;
using AspectOrientedProgramming.API.Logging;
using Castle.DynamicProxy;
using System.Reflection;

namespace AspectOrientedProgramming.API.Aspects;

/// <summary>
/// Interceptor that logs method execution information with detailed timestamps and correlation tracking
/// </summary>
public class LoggingInterceptor(
    ILogger<LoggingInterceptor> logger
) : BaseInterceptor
{
    private static readonly AsyncLocal<string> _correlationId = new();

    /// <summary>
    /// Gets or sets the current correlation ID for request tracking
    /// </summary>
    public static string CorrelationId
    {
        get => _correlationId.Value ??= Guid.NewGuid().ToString("N")[..8];
        set => _correlationId.Value = value;
    }

    /// <summary>
    /// Determines whether the method should be intercepted for logging
    /// </summary>
    /// <param name="invocation">The method invocation context</param>
    /// <returns>True if the method should be logged, false otherwise</returns>
    public override bool ShouldIntercept(IInvocation invocation)
    {
        // Check if method has Log attribute
        return invocation.Method.GetCustomAttribute<LogAttribute>() != null;
    }

    /// <summary>
    /// Executed before the method invocation
    /// </summary>
    /// <param name="invocation">The method invocation context</param>
    protected override void OnBefore(IInvocation invocation)
    {
        var methodName = GetMethodName(invocation.Method);
        var arguments = string.Join(", ", invocation.Arguments.Select(a => (a ?? "null")?.ToString()).ToArray());
        
        logger.LogInformation(LogMessageTemplates.MethodExecuting,
            CorrelationId,
            DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"),
            methodName, 
            arguments);
    }

    /// <summary>
    /// Executed after the method invocation if it was successful
    /// </summary>
    /// <param name="invocation">The method invocation context</param>
    protected override void OnAfterSuccess(IInvocation invocation)
    {
        var methodName = GetMethodName(invocation.Method);
        var result = invocation.ReturnValue?.ToString() ?? "void";
        
        logger.LogInformation(LogMessageTemplates.MethodExecuted,
            CorrelationId,
            DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"),
            methodName, 
            "N/A", // Execution time is tracked by PerformanceInterceptor
            result);
    }

    /// <summary>
    /// Executed if an exception occurs during method invocation
    /// </summary>
    /// <param name="invocation">The method invocation context</param>
    /// <param name="exception">The exception that occurred</param>
    protected override void OnException(IInvocation invocation, Exception exception)
    {
        var methodName = GetMethodName(invocation.Method);
        
        logger.LogError(exception, LogMessageTemplates.MethodException,
            CorrelationId,
            DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"),
            methodName,
            "N/A"); // Execution time is tracked by PerformanceInterceptor
    }
}
