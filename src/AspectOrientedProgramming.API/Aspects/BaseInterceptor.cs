using Castle.DynamicProxy;
using System.Reflection;

namespace AspectOrientedProgramming.API.Aspects;

/// <summary>
/// Base interceptor class that provides common functionality for all interceptors
/// </summary>
public abstract class BaseInterceptor : ICustomInterceptor
{
    /// <summary>
    /// Main interception method that is called for each method invocation
    /// </summary>
    /// <param name="invocation">The method invocation context</param>
    public virtual void Intercept(IInvocation invocation)
    {
        // Check if the method should be intercepted
        if (!ShouldIntercept(invocation))
        {
            invocation.Proceed();
            return;
        }

        // Execute before logic
        OnBefore(invocation);

        try
        {
            // Proceed with the original method execution
            invocation.Proceed();

            // Execute after success logic
            OnAfterSuccess(invocation);
        }
        catch (Exception ex)
        {
            // Execute exception handling logic
            OnException(invocation, ex);
            throw; // Re-throw the exception
        }
        finally
        {
            // Execute after logic (always executed)
            OnAfter(invocation);
        }
    }

    /// <summary>
    /// Determines whether the method should be intercepted
    /// </summary>
    /// <param name="invocation">The method invocation context</param>
    /// <returns>True if the method should be intercepted, false otherwise</returns>
    public virtual bool ShouldIntercept(IInvocation invocation)
    {
        return true;
    }

    /// <summary>
    /// Executed before the method invocation
    /// </summary>
    /// <param name="invocation">The method invocation context</param>
    protected virtual void OnBefore(IInvocation invocation)
    {
        // Override in derived classes to add before logic
    }

    /// <summary>
    /// Executed after the method invocation if it was successful
    /// </summary>
    /// <param name="invocation">The method invocation context</param>
    protected virtual void OnAfterSuccess(IInvocation invocation)
    {
        // Override in derived classes to add after success logic
    }

    /// <summary>
    /// Executed if an exception occurs during method invocation
    /// </summary>
    /// <param name="invocation">The method invocation context</param>
    /// <param name="exception">The exception that occurred</param>
    protected virtual void OnException(IInvocation invocation, Exception exception)
    {
        // Override in derived classes to add exception handling logic
    }

    /// <summary>
    /// Executed after the method invocation (always executed)
    /// </summary>
    /// <param name="invocation">The method invocation context</param>
    protected virtual void OnAfter(IInvocation invocation)
    {
        // Override in derived classes to add after logic
    }

    /// <summary>
    /// Gets the name of the method being invoked
    /// </summary>
    /// <param name="method">The method info</param>
    /// <returns>The method name</returns>
    protected string GetMethodName(MethodInfo method)
    {
        return $"{method.DeclaringType?.Name}.{method.Name}";
    }
}
