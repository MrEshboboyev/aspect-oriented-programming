using Castle.DynamicProxy;

namespace AspectOrientedProgramming.API.Aspects;

/// <summary>
/// Factory for creating proxy objects with interceptors
/// </summary>
public static class ProxyFactory
{
    private static readonly ProxyGenerator _proxyGenerator = new();

    /// <summary>
    /// Creates a proxy for the specified interface with the given interceptors
    /// </summary>
    /// <typeparam name="TInterface">The interface type to proxy</typeparam>
    /// <param name="target">The target object to wrap</param>
    /// <param name="interceptors">The interceptors to apply</param>
    /// <returns>A proxy object</returns>
    public static TInterface CreateProxy<TInterface>(object target, params IInterceptor[] interceptors) where TInterface : class
    {
        return _proxyGenerator.CreateInterfaceProxyWithTarget<TInterface>((TInterface)target, interceptors);
    }

    /// <summary>
    /// Registers a type with its proxy in the service collection
    /// </summary>
    /// <typeparam name="TInterface">The interface type</typeparam>
    /// <typeparam name="TImplementation">The implementation type</typeparam>
    /// <param name="services">The service collection</param>
    /// <param name="interceptorTypes">The interceptor types to apply</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddProxiedScoped<TInterface, TImplementation>(
        this IServiceCollection services,
        params Type[] interceptorTypes)
        where TInterface : class
        where TImplementation : class, TInterface
    {
        services.AddScoped<TImplementation>();
        services.AddScoped(provider =>
        {
            var target = provider.GetRequiredService<TImplementation>();
            var interceptors = interceptorTypes.Select(type => (IInterceptor)provider.GetRequiredService(type)).ToArray();
            return CreateProxy<TInterface>(target, interceptors);
        });

        // Register interceptor types
        foreach (var interceptorType in interceptorTypes)
        {
            if (!services.Any(x => x.ServiceType == interceptorType))
            {
                services.AddScoped(interceptorType);
            }
        }

        return services;
    }
}
