using AspectOrientedProgramming.API.Aspects;
using AspectOrientedProgramming.API.Attributes;
using AspectOrientedProgramming.API.Models;
using Castle.DynamicProxy;
using System.ComponentModel.DataAnnotations;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;

namespace AspectOrientedProgramming.API.Tests
{
    public class ValidationInterceptorTests
    {
        private ValidationInterceptor _interceptor;

        public ValidationInterceptorTests()
        {
            var mockLogger = new Mock<ILogger<ValidationInterceptor>>();
            _interceptor = new ValidationInterceptor(mockLogger.Object);
        }

        [Fact]
        public void ValidationInterceptor_Should_Validate_Valid_Model()
        {
            // Arrange
            var testTarget = new ValidationTestService();
            var proxyGenerator = new ProxyGenerator();
            var proxy = proxyGenerator.CreateInterfaceProxyWithTarget<IValidationTestService>(testTarget, _interceptor);
            var validProduct = new Product 
            { 
                Name = "Valid Product", 
                Description = "A valid product description", 
                Price = 10.99m, 
                Category = "Electronics" 
            };

            // Act & Assert
            var exception = Record.Exception(() => proxy.ProcessProduct(validProduct));
            Assert.Null(exception);
        }

        [Fact]
        public void ValidationInterceptor_Should_Throw_Exception_For_Invalid_Model()
        {
            // Arrange
            var testTarget = new ValidationTestService();
            var proxyGenerator = new ProxyGenerator();
            var proxy = proxyGenerator.CreateInterfaceProxyWithTarget<IValidationTestService>(testTarget, _interceptor);
            var invalidProduct = new Product 
            { 
                Name = "", // Required field
                Description = "A valid product description", 
                Price = -5.00m, // Invalid price
                Category = "" // Required field
            };

            // Act & Assert
            var exception = Record.Exception(() => proxy.ProcessProduct(invalidProduct));
            Assert.NotNull(exception);
            Assert.IsType<ValidationException>(exception);
        }

        [Fact]
        public void ValidationInterceptor_Should_Determine_If_Method_Should_Be_Intercepted()
        {
            // Arrange
            var invocation = new Mock<Castle.DynamicProxy.IInvocation>();
            var method = typeof(ValidationTestService).GetMethod(nameof(ValidationTestService.ProcessProduct));
            invocation.Setup(i => i.Method).Returns(method);

            // Act
            var shouldIntercept = _interceptor.ShouldIntercept(invocation.Object);

            // Assert
            Assert.True(shouldIntercept);
        }

        [Fact]
        public void ValidationInterceptor_Should_Not_Intercept_Methods_Without_Validate_Attribute()
        {
            // Arrange
            var invocation = new Mock<Castle.DynamicProxy.IInvocation>();
            var method = typeof(ValidationTestService).GetMethod(nameof(ValidationTestService.GetData));
            invocation.Setup(i => i.Method).Returns(method);

            // Act
            var shouldIntercept = _interceptor.ShouldIntercept(invocation.Object);

            // Assert
            Assert.False(shouldIntercept);
        }
    }

    public interface IValidationTestService
    {
        string GetData();
        
        [Validate]
        string ProcessProduct(Product product);
    }

    public class ValidationTestService : IValidationTestService
    {
        public string GetData()
        {
            return "Test Data";
        }

        [Validate]
        public string ProcessProduct(Product product)
        {
            return $"Processed product: {product.Name}";
        }
    }
}