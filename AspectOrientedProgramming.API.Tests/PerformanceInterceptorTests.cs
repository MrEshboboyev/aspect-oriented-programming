using AspectOrientedProgramming.API.Aspects;
using AspectOrientedProgramming.API.Attributes;
using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using Moq;

namespace AspectOrientedProgramming.API.Tests;

public class PerformanceInterceptorTests
{
    private Mock<ILogger<PerformanceInterceptor>> _mockLogger;
    private PerformanceInterceptor _interceptor;

    public PerformanceInterceptorTests()
    {
        _mockLogger = new Mock<ILogger<PerformanceInterceptor>>();
        _interceptor = new PerformanceInterceptor(_mockLogger.Object);
    }

    [Fact]
    public void PerformanceInterceptor_Should_Monitor_Method_Execution_Time()
    {
        // Arrange
        var testTarget = new PerformanceTestService();
        var proxyGenerator = new ProxyGenerator();
        var proxy = proxyGenerator.CreateInterfaceProxyWithTarget<IPerformanceTestService>(testTarget, _interceptor);

        // Act
        var result = proxy.GetPerformanceData();

        // Assert
        Assert.Equal("Performance Data", result);
        // In a real test, we would verify that logging occurred
    }

    [Fact]
    public void PerformanceInterceptor_Should_Determine_If_Method_Should_Be_Intercepted()
    {
        // Arrange
        var invocation = new Mock<Castle.DynamicProxy.IInvocation>();
        var method = typeof(PerformanceTestService).GetMethod(nameof(PerformanceTestService.GetPerformanceData));
        invocation.Setup(i => i.Method).Returns(method);

        // Act
        var shouldIntercept = _interceptor.ShouldIntercept(invocation.Object);

        // Assert
        Assert.True(shouldIntercept);
    }

    [Fact]
    public void PerformanceInterceptor_Should_Not_Intercept_Methods_Without_Performance_Attribute()
    {
        // Arrange
        var invocation = new Mock<Castle.DynamicProxy.IInvocation>();
        var method = typeof(PerformanceTestService).GetMethod(nameof(PerformanceTestService.GetData));
        invocation.Setup(i => i.Method).Returns(method);

        // Act
        var shouldIntercept = _interceptor.ShouldIntercept(invocation.Object);

        // Assert
        Assert.False(shouldIntercept);
    }
}

public interface IPerformanceTestService
{
    string GetData();
    
    [Performance(1000)]
    string GetPerformanceData();
}

public class PerformanceTestService : IPerformanceTestService
{
    public string GetData()
    {
        return "Test Data";
    }

    [Performance(1000)]
    public string GetPerformanceData()
    {
        return "Performance Data";
    }
}
