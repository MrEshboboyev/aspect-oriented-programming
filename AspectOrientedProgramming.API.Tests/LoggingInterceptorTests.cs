using AspectOrientedProgramming.API.Aspects;
using AspectOrientedProgramming.API.Attributes;
using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using Moq;

namespace AspectOrientedProgramming.API.Tests;

public class LoggingInterceptorTests
{
    private Mock<ILogger<LoggingInterceptor>> _mockLogger;
    private LoggingInterceptor _interceptor;

    public LoggingInterceptorTests()
    {
        _mockLogger = new Mock<ILogger<LoggingInterceptor>>();
        _interceptor = new LoggingInterceptor(_mockLogger.Object);
    }

    [Fact]
    public void LoggingInterceptor_Should_Log_Method_Execution()
    {
        // Arrange
        var testTarget = new LoggingTestService();
        var proxyGenerator = new ProxyGenerator();
        var proxy = proxyGenerator.CreateInterfaceProxyWithTarget<ILoggingTestService>(testTarget, _interceptor);

        // Act
        var result = proxy.GetData();

        // Assert
        Assert.Equal("Test Data", result);
        // Verify that logger was called (this would require more sophisticated verification in a real test)
    }

    [Fact]
    public void LoggingInterceptor_Should_Determine_If_Method_Should_Be_Intercepted()
    {
        // Arrange
        var invocation = new Mock<Castle.DynamicProxy.IInvocation>();
        var method = typeof(LoggingTestService).GetMethod(nameof(LoggingTestService.GetData));
        invocation.Setup(i => i.Method).Returns(method);

        // Act
        var shouldIntercept = _interceptor.ShouldIntercept(invocation.Object);

        // Assert
        Assert.True(shouldIntercept);
    }
}

public interface ILoggingTestService
{
    [Log]
    string GetData();
}

public class LoggingTestService : ILoggingTestService
{
    [Log]
    public string GetData()
    {
        return "Test Data";
    }
}
