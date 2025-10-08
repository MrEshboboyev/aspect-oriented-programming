using AspectOrientedProgramming.API.Attributes;
using AspectOrientedProgramming.API.Logging;
using Castle.DynamicProxy;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace AspectOrientedProgramming.API.Aspects;

/// <summary>
/// Interceptor that validates method arguments using Data Annotations with detailed logging
/// </summary>
public class ValidationInterceptor(
    ILogger<ValidationInterceptor> logger
) : BaseInterceptor
{
    /// <summary>
    /// Determines whether the method should be intercepted for validation
    /// </summary>
    /// <param name="invocation">The method invocation context</param>
    /// <returns>True if the method should be validated, false otherwise</returns>
    public override bool ShouldIntercept(IInvocation invocation)
    {
        // Check if method has Validate attribute
        return invocation.Method.GetCustomAttribute<ValidateAttribute>() != null;
    }

    /// <summary>
    /// Executed before the method invocation to validate arguments
    /// </summary>
    /// <param name="invocation">The method invocation context</param>
    protected override void OnBefore(IInvocation invocation)
    {
        var methodName = GetMethodName(invocation.Method);
        logger.LogDebug(LogMessageTemplates.ValidationStarted,
            LoggingInterceptor.CorrelationId,
            DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"),
            methodName);
        
        // Validate each argument
        for (int i = 0; i < invocation.Arguments.Length; i++)
        {
            var argument = invocation.Arguments[i];
            if (argument != null)
            {
                var validationContext = new ValidationContext(argument);
                var validationResults = new List<ValidationResult>();
                
                if (!Validator.TryValidateObject(argument, validationContext, validationResults, true))
                {
                    var errorMessage = string.Join("; ", validationResults.Select(r => r.ErrorMessage));
                    logger.LogWarning(LogMessageTemplates.ValidationFailed,
                        LoggingInterceptor.CorrelationId,
                        DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                        i, 
                        methodName, 
                        errorMessage);
                    
                    throw new ValidationException($"Validation failed for argument {i}: {errorMessage}");
                }
            }
        }
        
        logger.LogInformation(LogMessageTemplates.ValidationCompleted,
            LoggingInterceptor.CorrelationId,
            DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"),
            methodName);
    }
}
