using AspectOrientedProgramming.API.Attributes;
using AspectOrientedProgramming.API.Logging;
using Castle.DynamicProxy;
using System.Reflection;

namespace AspectOrientedProgramming.API.Aspects;

/// <summary>
/// Interceptor that provides security checks for method execution with detailed logging
/// </summary>
public class SecurityInterceptor(
    IHttpContextAccessor httpContextAccessor,
    ILogger<SecurityInterceptor> logger
) : BaseInterceptor
{
    /// <summary>
    /// Determines whether the method should be intercepted based on security requirements
    /// </summary>
    /// <param name="invocation">The method invocation context</param>
    /// <returns>True if the method should be intercepted, false otherwise</returns>
    public override bool ShouldIntercept(IInvocation invocation)
    {
        // Check if method has Authorize attribute
        return invocation.Method.GetCustomAttribute<AuthorizeAttribute>() != null;
    }

    /// <summary>
    /// Executed before the method invocation to perform security checks
    /// </summary>
    /// <param name="invocation">The method invocation context</param>
    protected override void OnBefore(IInvocation invocation)
    {
        var methodName = GetMethodName(invocation.Method);
        var httpContext = httpContextAccessor.HttpContext;
        
        logger.LogDebug(LogMessageTemplates.SecurityCheckStarted,
            LoggingInterceptor.CorrelationId,
            DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"),
            methodName);
        
        if (httpContext == null)
        {
            logger.LogWarning(LogMessageTemplates.SecurityCheckSkipped,
                LoggingInterceptor.CorrelationId,
                DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                methodName);
            return;
        }

        // Check if user is authenticated
        if (httpContext.User?.Identity?.IsAuthenticated == false)
        {
            logger.LogWarning(LogMessageTemplates.UnauthorizedAccess,
                LoggingInterceptor.CorrelationId,
                DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                methodName, 
                httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown");
            // In a real application, you might throw an UnauthorizedAccessException here
            // throw new UnauthorizedAccessException($"Access to {methodName} requires authentication");
        }
        else
        {
            logger.LogInformation(LogMessageTemplates.AuthorizedAccess,
                LoggingInterceptor.CorrelationId,
                DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                methodName, 
                httpContext.User?.Identity?.Name ?? "Unknown",
                httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown");
            
            // Check for role requirements
            var authorizeAttribute = invocation.Method.GetCustomAttribute<AuthorizeAttribute>();
            if (authorizeAttribute != null && !string.IsNullOrEmpty(authorizeAttribute.Roles))
            {
                var requiredRoles = authorizeAttribute.Roles.Split(',');
                var hasRequiredRole = requiredRoles.Any(role => httpContext.User?.IsInRole(role.Trim()) == true);
                
                if (!hasRequiredRole)
                {
                    logger.LogWarning(LogMessageTemplates.InsufficientRoles,
                        LoggingInterceptor.CorrelationId,
                        DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                        httpContext.User?.Identity?.Name ?? "Unknown", 
                        authorizeAttribute.Roles, 
                        methodName);
                    // In a real application, you might throw a ForbiddenAccessException here
                    // throw new ForbiddenAccessException($"Access to {methodName} requires roles: {authorizeAttribute.Roles}");
                }
            }
        }
    }
}
