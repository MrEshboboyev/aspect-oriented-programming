using Castle.DynamicProxy;

namespace AspectOrientedProgramming.API.Aspects;

/// <summary>
/// Custom interceptor interface that extends Castle DynamicProxy's IInterceptor
/// </summary>
public interface ICustomInterceptor : IInterceptor
{
}
