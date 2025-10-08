# Aspect-Oriented Programming API

This is a .NET Web API project that demonstrates Aspect-Oriented Programming (AOP) concepts using Castle DynamicProxy.

## Overview

This project implements several cross-cutting concerns as aspects:
- **Logging**: Automatic method execution logging
- **Caching**: Automatic result caching based on method parameters
- **Validation**: Automatic parameter validation using Data Annotations
- **Security**: Authorization checks for method execution
- **Performance Monitoring**: Method execution time monitoring

## Key Components

### Aspects
- [BaseInterceptor.cs](Aspects/BaseInterceptor.cs) - Base interceptor class
- [LoggingInterceptor.cs](Aspects/LoggingInterceptor.cs) - Logging aspect
- [CachingInterceptor.cs](Aspects/CachingInterceptor.cs) - Caching aspect
- [ValidationInterceptor.cs](Aspects/ValidationInterceptor.cs) - Validation aspect
- [SecurityInterceptor.cs](Aspects/SecurityInterceptor.cs) - Security aspect
- [PerformanceInterceptor.cs](Aspects/PerformanceInterceptor.cs) - Performance monitoring aspect

### Attributes
- [LogAttribute.cs](Attributes/LogAttribute.cs) - Enables logging for methods
- [CacheAttribute.cs](Attributes/CacheAttribute.cs) - Enables caching for methods
- [ValidateAttribute.cs](Attributes/ValidateAttribute.cs) - Enables validation for methods
- [AuthorizeAttribute.cs](Attributes/AuthorizeAttribute.cs) - Enables authorization for methods
- [PerformanceAttribute.cs](Attributes/PerformanceAttribute.cs) - Enables performance monitoring for methods

### Services
- [ProductService.cs](Services/ProductService.cs) - Product management with AOP
- [OrderService.cs](Services/OrderService.cs) - Order management with AOP

### Controllers
- [ProductsController.cs](Controllers/ProductsController.cs) - Product API endpoints
- [OrdersController.cs](Controllers/OrdersController.cs) - Order API endpoints

## Running the Project

```bash
dotnet run
```

The API will be available at:
- https://localhost:5001
- http://localhost:5000

## Example Usage

```csharp
public class ProductService : IProductService
{
    [Cache(10)] // Cache for 10 minutes
    [Log]       // Log method execution
    public List<Product> GetAllProducts()
    {
        // Implementation automatically cached and logged
    }

    [Validate]  // Validate input parameters
    [Log]       // Log method execution
    public Product CreateProduct(Product product)
    {
        // Product parameter automatically validated
        // Method execution automatically logged
    }
}
```