using AspectOrientedProgramming.API.Aspects;
using AspectOrientedProgramming.API.Attributes;
using Castle.DynamicProxy;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace AspectOrientedProgramming.API.Tests;

public class CacheInvalidationTests
{
    [Fact]
    public void CacheInvalidation_Should_Remove_Cached_Entries()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<CachingInterceptor>>();
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var interceptor = new CachingInterceptor(memoryCache, mockLogger.Object);
        
        var testService = new TestCacheService();
        var proxyGenerator = new ProxyGenerator();
        var proxy = proxyGenerator.CreateInterfaceProxyWithTarget<ITestCacheService>(testService, interceptor);
        
        // Act & Assert
        // First call should execute the method and cache the result
        var result1 = proxy.GetCachedData(1);
        Assert.Equal("Data 1", result1);
        
        // Second call should return cached result
        var result2 = proxy.GetCachedData(1);
        Assert.Equal("Data 1", result2);
        
        // Update data to trigger cache invalidation
        proxy.UpdateData(1, "Updated Data 1");
        
        // Next call should execute the method again (cache was invalidated)
        var result3 = proxy.GetCachedData(1);
        Assert.Equal("Updated Data 1", result3);
    }
    
    [Fact]
    public void CacheInvalidation_Should_Work_With_Patterns()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<CachingInterceptor>>();
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var interceptor = new CachingInterceptor(memoryCache, mockLogger.Object);
        
        var testService = new TestCacheService();
        var proxyGenerator = new ProxyGenerator();
        var proxy = proxyGenerator.CreateInterfaceProxyWithTarget<ITestCacheService>(testService, interceptor);
        
        // Act & Assert
        // Cache some data
        var result1 = proxy.GetCachedData(1);
        var result2 = proxy.GetCachedData(2);
        
        // Update all data to trigger bulk cache invalidation
        proxy.UpdateAllData();
        
        // Next calls should execute the method again (cache was invalidated)
        var result3 = proxy.GetCachedData(1);
        var result4 = proxy.GetCachedData(2);
        
        Assert.Equal("Data 1 - Updated", result3);
        Assert.Equal("Data 2 - Updated", result4);
    }
}

public interface ITestCacheService
{
    [Cache(5)]
    string GetCachedData(int id);
    
    [InvalidateCache("ITestCacheService.GetCachedData({id})")]
    void UpdateData(int id, string newData);
    
    [InvalidateCache("*")]
    void UpdateAllData();
}

public class TestCacheService : ITestCacheService
{
    private Dictionary<int, string> _data = new()
    {
        { 1, "Data 1" },
        { 2, "Data 2" }
    };
    
    [Cache(5)]
    public string GetCachedData(int id)
    {
        return _data.TryGetValue(id, out var value) ? value : "Not Found";
    }
    
    public void UpdateData(int id, string newData)
    {
        _data[id] = newData;
    }
    
    public void UpdateAllData()
    {
        for (int i = 1; i <= _data.Count; i++)
        {
            _data[i] = $"Data {i} - Updated";
        }
    }
}