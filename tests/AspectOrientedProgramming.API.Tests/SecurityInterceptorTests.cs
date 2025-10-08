using AspectOrientedProgramming.API.Aspects;
using AspectOrientedProgramming.API.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace AspectOrientedProgramming.API.Tests;

public class SecurityInterceptorTests
{
    private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private Mock<ILogger<SecurityInterceptor>> _mockLogger;
    private SecurityInterceptor _interceptor;

    public SecurityInterceptorTests()
    {
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _mockLogger = new Mock<ILogger<SecurityInterceptor>>();
        _interceptor = new SecurityInterceptor(_mockHttpContextAccessor.Object, _mockLogger.Object);
    }

    [Fact]
    public void SecurityInterceptor_Should_Determine_If_Method_Should_Be_Intercepted()
    {
        // Arrange
        var invocation = new Mock<Castle.DynamicProxy.IInvocation>();
        var method = typeof(SecurityTestService).GetMethod(nameof(SecurityTestService.GetSecureData));
        invocation.Setup(i => i.Method).Returns(method);

        // Act
        var shouldIntercept = _interceptor.ShouldIntercept(invocation.Object);

        // Assert
        Assert.True(shouldIntercept);
    }

    [Fact]
    public void SecurityInterceptor_Should_Not_Intercept_Methods_Without_Authorize_Attribute()
    {
        // Arrange
        var invocation = new Mock<Castle.DynamicProxy.IInvocation>();
        var method = typeof(SecurityTestService).GetMethod(nameof(SecurityTestService.GetData));
        invocation.Setup(i => i.Method).Returns(method);

        // Act
        var shouldIntercept = _interceptor.ShouldIntercept(invocation.Object);

        // Assert
        Assert.False(shouldIntercept);
    }
}

public interface ISecurityTestService
{
    string GetData();
    
    [Authorize]
    string GetSecureData();
}

public class SecurityTestService : ISecurityTestService
{
    public string GetData()
    {
        return "Test Data";
    }

    [Authorize]
    public string GetSecureData()
    {
        return "Secure Data";
    }
}
