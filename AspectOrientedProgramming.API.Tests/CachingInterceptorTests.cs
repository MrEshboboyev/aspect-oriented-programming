using AspectOrientedProgramming.API.Aspects;
using AspectOrientedProgramming.API.Attributes;
using Castle.DynamicProxy;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace AspectOrientedProgramming.API.Tests
{
    public class CachingInterceptorTests
    {
        private IMemoryCache _memoryCache;
        private CachingInterceptor _interceptor;

        public CachingInterceptorTests()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            var mockLogger = new Mock<ILogger<CachingInterceptor>>();
            _interceptor = new CachingInterceptor(_memoryCache, mockLogger.Object);
        }

        [Fact]
        public void CachingInterceptor_Should_Cache_Method_Results()
        {
            // Arrange
            var testTarget = new CachingTestService();
            var proxyGenerator = new ProxyGenerator();
            var proxy = proxyGenerator.CreateInterfaceProxyWithTarget<ICachingTestService>(testTarget, _interceptor);

            // Act
            var result1 = proxy.GetCachedData();
            var result2 = proxy.GetCachedData();

            // Assert
            Assert.Equal(result1, result2);
            // We'll just check that the results are the same, not the exact string value
        }

        [Fact]
        public void CachingInterceptor_Should_Determine_If_Method_Should_Be_Intercepted()
        {
            // Arrange
            var invocation = new Mock<Castle.DynamicProxy.IInvocation>();
            var method = typeof(CachingTestService).GetMethod(nameof(CachingTestService.GetCachedData));
            invocation.Setup(i => i.Method).Returns(method);

            // Act
            var shouldIntercept = _interceptor.ShouldIntercept(invocation.Object);

            // Assert
            Assert.True(shouldIntercept);
        }

        [Fact]
        public void CachingInterceptor_Should_Not_Intercept_Methods_Without_Cache_Attribute()
        {
            // Arrange
            var invocation = new Mock<Castle.DynamicProxy.IInvocation>();
            var method = typeof(CachingTestService).GetMethod(nameof(CachingTestService.GetData));
            invocation.Setup(i => i.Method).Returns(method);

            // Act
            var shouldIntercept = _interceptor.ShouldIntercept(invocation.Object);

            // Assert
            Assert.False(shouldIntercept);
        }
    }

    public interface ICachingTestService
    {
        string GetData();
        
        [Cache(1)]
        string GetCachedData();
    }

    public class CachingTestService : ICachingTestService
    {
        private int _callCount = 0;

        public string GetData()
        {
            return "Test Data";
        }

        [Cache(1)]
        public string GetCachedData()
        {
            _callCount++;
            return $"Cached Data {_callCount}";
        }
    }
}