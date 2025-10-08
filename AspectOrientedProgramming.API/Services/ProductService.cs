using AspectOrientedProgramming.API.Models;

namespace AspectOrientedProgramming.API.Services;

/// <summary>
/// Implementation of product service operations with detailed logging
/// </summary>
public class ProductService(
    ILogger<ProductService> logger
) : IProductService
{
    private static readonly List<Product> _products = new List<Product>();
    private static int _nextId = 1;

    /// <inheritdoc />
    public List<Product> GetAllProducts()
    {
        logger.LogDebug("GetAllProducts method called");
        
        // Simulate some processing time
        Thread.Sleep(100);
        logger.LogInformation("Retrieved {ProductCount} products from storage", _products.Count);
        return new List<Product>(_products);
    }

    /// <inheritdoc />
    public Product? GetProductById(int id)
    {
        logger.LogDebug("GetProductById method called with ID: {ProductId}", id);
        
        // Simulate some processing time
        Thread.Sleep(50);
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product != null)
        {
            logger.LogInformation("Found product with ID: {ProductId}, Name: {ProductName}", id, product.Name);
        }
        else
        {
            logger.LogWarning("Product with ID: {ProductId} not found", id);
        }
        return product;
    }

    /// <inheritdoc />
    public Product CreateProduct(Product product)
    {
        logger.LogInformation("Creating new product: {ProductName}", product.Name);
        
        product.Id = _nextId++;
        product.CreatedAt = DateTime.UtcNow;
        product.UpdatedAt = DateTime.UtcNow;
        _products.Add(product);
        
        logger.LogInformation("Product {ProductName} created successfully with ID {ProductId}", product.Name, product.Id);
        return product;
    }

    /// <inheritdoc />
    public Product UpdateProduct(int id, Product product)
    {
        logger.LogInformation("Updating product with ID: {ProductId}", id);
        
        var existingProduct = _products.FirstOrDefault(p => p.Id == id) 
            ?? throw new ArgumentException($"Product with ID {id} not found");
        
        logger.LogDebug("Found existing product: {ProductName} (ID: {ProductId})", existingProduct.Name, existingProduct.Id);
        
        existingProduct.Name = product.Name;
        existingProduct.Description = product.Description;
        existingProduct.Price = product.Price;
        existingProduct.Category = product.Category;
        existingProduct.UpdatedAt = DateTime.UtcNow;
        
        logger.LogInformation("Product {ProductName} (ID: {ProductId}) updated successfully", product.Name, id);
        return existingProduct;
    }

    /// <inheritdoc />
    public void DeleteProduct(int id)
    {
        logger.LogInformation("Deleting product with ID: {ProductId}", id);
        
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product != null)
        {
            _products.Remove(product);
            logger.LogInformation("Product {ProductName} (ID: {ProductId}) deleted successfully", product.Name, id);
        }
        else
        {
            logger.LogWarning("Attempted to delete non-existent product with ID: {ProductId}", id);
        }
    }

    /// <inheritdoc />
    public List<Product> GetProductsByCategory(string category)
    {
        logger.LogDebug("GetProductsByCategory method called with category: {Category}", category);
        
        // Simulate some processing time
        Thread.Sleep(75);
        var filteredProducts = new List<Product>(_products.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase)));
        logger.LogInformation("Found {ProductCount} products in category: {Category}", filteredProducts.Count, category);
        return filteredProducts;
    }

    /// <summary>
    /// Seeds the product data with sample products
    /// </summary>
    public void SeedData()
    {
        if (_products.Count == 0)
        {
            logger.LogInformation("Seeding initial product data");
            
            _products.AddRange(new List<Product>
            {
                new Product { Id = _nextId++, Name = "Laptop", Description = "High-performance laptop", Price = 1200.00m, Category = "Electronics", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Product { Id = _nextId++, Name = "Smartphone", Description = "Latest smartphone model", Price = 800.00m, Category = "Electronics", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Product { Id = _nextId++, Name = "Coffee Maker", Description = "Automatic coffee maker", Price = 150.00m, Category = "Home Appliances", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Product { Id = _nextId++, Name = "Running Shoes", Description = "Comfortable running shoes", Price = 120.00m, Category = "Sports", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Product { Id = _nextId++, Name = "Book: C# Programming", Description = "Learn C# programming", Price = 45.00m, Category = "Books", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            });
            
            logger.LogInformation("Seeded {ProductCount} sample products", _products.Count);
        }
        else
        {
            logger.LogDebug("Product data already seeded, skipping");
        }
    }
}
