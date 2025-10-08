using AspectOrientedProgramming.API.Aspects;
using AspectOrientedProgramming.API.Logging;

namespace AspectOrientedProgramming.API.Middleware;

/// <summary>
/// Middleware to set correlation IDs for request tracking
/// </summary>
public class CorrelationIdMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ILogger<CorrelationIdMiddleware> logger)
    {
        // Set a correlation ID for this request
        var correlationId = Guid.NewGuid().ToString("N")[..8];
        LoggingInterceptor.CorrelationId = correlationId;
        
        // Add correlation ID to response headers
        context.Response.Headers.Append("X-Correlation-Id", correlationId);
        
        logger.LogInformation(LogMessageTemplates.HttpRequestStarted,
            correlationId,
            DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"),
            context.Request.Method,
            context.Request.Path,
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown");
        
        await next(context);
        
        logger.LogInformation(LogMessageTemplates.HttpRequestCompleted,
            correlationId,
            DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"),
            context.Request.Method,
            context.Request.Path);
    }
}
