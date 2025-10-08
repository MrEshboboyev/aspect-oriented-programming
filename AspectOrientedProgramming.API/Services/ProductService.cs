using AspectOrientedProgramming.API.Attributes;
using AspectOrientedProgramming.API.Models;
using System.Diagnostics;

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

    /// <summary>
    /// Gets all products with caching
    /// </summary>
    /// <returns>List of all products</returns>
    [Cache(10)] // Cache for 10 minutes
    [Log]
    [Performance(500)] // Warn if takes more than 500ms
    public List<Product> GetAllProducts()
    {
        logger.LogDebug("GetAllProducts method called");
        
        // Simulate some processing time
        Thread.Sleep(100);
        logger.LogInformation("Retrieved {ProductCount} products from storage", _products.Count);
        return new List<Product>(_products);
    }

    /// <summary>
    /// Gets a product by its ID with caching
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>Product with the specified ID, or null if not found</returns>
    [Cache(5)] // Cache for 5 minutes
    [Log]
    [Performance(200)] // Warn if takes more than 200ms
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

    /// <summary>
    /// Creates a new product with validation
    /// </summary>
    /// <param name="product">Product to create</param>
    /// <returns>Created product</returns>
    [Validate]
    [Log]
    [Performance(1000)] // Warn if takes more than 1 second
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

    /// <summary>
    /// Updates an existing product with validation
    /// </summary>
    /// <param name="id">ID of the product to update</param>
    /// <param name="product">Updated product data</param>
    /// <returns>Updated product</returns>
    [Validate]
    [Log]
    [Performance(1000)] // Warn if takes more than 1 second
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

    /// <summary>
    /// Deletes a product
    /// </summary>
    /// <param name="id">ID of the product to delete</param>
    [Log]
    [Performance(500)] // Warn if takes more than 500ms
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

    /// <summary>
    /// Gets products by category with caching
    /// </summary>
    /// <param name="category">Category to filter by</param>
    /// <returns>List of products in the specified category</returns>
    [Cache(5)] // Cache for 5 minutes
    [Log]
    [Performance(300)] // Warn if takes more than 300ms
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