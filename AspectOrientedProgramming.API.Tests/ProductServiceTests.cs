using AspectOrientedProgramming.API.Models;
using AspectOrientedProgramming.API.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace AspectOrientedProgramming.API.Tests;

public class ProductServiceTests
{
    private Mock<ILogger<ProductService>> _mockLogger;
    private ProductService _productService;

    public ProductServiceTests()
    {
        _mockLogger = new Mock<ILogger<ProductService>>();
        _productService = new ProductService(_mockLogger.Object);
    }

    [Fact]
    public void ProductService_Should_Create_Product()
    {
        // Arrange
        var product = new Product
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 10.99m,
            Category = "Test Category"
        };

        // Act
        var createdProduct = _productService.CreateProduct(product);

        // Assert
        Assert.NotNull(createdProduct);
        Assert.Equal(product.Name, createdProduct.Name);
        Assert.Equal(product.Description, createdProduct.Description);
        Assert.Equal(product.Price, createdProduct.Price);
        Assert.Equal(product.Category, createdProduct.Category);
        Assert.True(createdProduct.Id > 0);
    }

    [Fact]
    public void ProductService_Should_Get_Product_By_Id()
    {
        // Arrange
        var product = new Product
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 10.99m,
            Category = "Test Category"
        };
        var createdProduct = _productService.CreateProduct(product);

        // Act
        var retrievedProduct = _productService.GetProductById(createdProduct.Id);

        // Assert
        Assert.NotNull(retrievedProduct);
        Assert.Equal(createdProduct.Id, retrievedProduct.Id);
        Assert.Equal(createdProduct.Name, retrievedProduct.Name);
    }

    [Fact]
    public void ProductService_Should_Return_Null_For_NonExistent_Product()
    {
        // Act
        var product = _productService.GetProductById(999);

        // Assert
        Assert.Null(product);
    }

    [Fact]
    public void ProductService_Should_Update_Product()
    {
        // Arrange
        var product = new Product
        {
            Name = "Original Product",
            Description = "Original Description",
            Price = 10.99m,
            Category = "Original Category"
        };
        var createdProduct = _productService.CreateProduct(product);

        var updatedProduct = new Product
        {
            Name = "Updated Product",
            Description = "Updated Description",
            Price = 20.99m,
            Category = "Updated Category"
        };

        // Act
        var result = _productService.UpdateProduct(createdProduct.Id, updatedProduct);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createdProduct.Id, result.Id);
        Assert.Equal(updatedProduct.Name, result.Name);
        Assert.Equal(updatedProduct.Description, result.Description);
        Assert.Equal(updatedProduct.Price, result.Price);
        Assert.Equal(updatedProduct.Category, result.Category);
    }

    [Fact]
    public void ProductService_Should_Throw_Exception_For_NonExistent_Product_Update()
    {
        // Arrange
        var product = new Product
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 10.99m,
            Category = "Test Category"
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _productService.UpdateProduct(999, product));
        Assert.Contains("not found", exception.Message);
    }

    [Fact]
    public void ProductService_Should_Delete_Product()
    {
        // Arrange
        var product = new Product
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 10.99m,
            Category = "Test Category"
        };
        var createdProduct = _productService.CreateProduct(product);

        // Act
        _productService.DeleteProduct(createdProduct.Id);

        // Assert
        var retrievedProduct = _productService.GetProductById(createdProduct.Id);
        Assert.Null(retrievedProduct);
    }

    [Fact]
    public void ProductService_Should_Get_Products_By_Category()
    {
        // Arrange
        var product1 = new Product
        {
            Name = "Product 1",
            Description = "Description 1",
            Price = 10.99m,
            Category = "Electronics"
        };
        var product2 = new Product
        {
            Name = "Product 2",
            Description = "Description 2",
            Price = 20.99m,
            Category = "Electronics"
        };
        var product3 = new Product
        {
            Name = "Product 3",
            Description = "Description 3",
            Price = 15.99m,
            Category = "Books"
        };

        _productService.CreateProduct(product1);
        _productService.CreateProduct(product2);
        _productService.CreateProduct(product3);

        // Act
        var electronicsProducts = _productService.GetProductsByCategory("Electronics");

        // Assert
        Assert.Equal(2, electronicsProducts.Count);
        Assert.All(electronicsProducts, p => Assert.Equal("Electronics", p.Category));
    }
}
