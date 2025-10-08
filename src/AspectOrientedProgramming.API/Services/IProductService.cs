using AspectOrientedProgramming.API.Attributes;
using AspectOrientedProgramming.API.Models;

namespace AspectOrientedProgramming.API.Services;

/// <summary>
/// Interface for product service operations
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Gets all products
    /// </summary>
    /// <returns>List of all products</returns>
    [Cache(10)] // Cache for 10 minutes
    [Log]
    [Performance(500)] // Warn if takes more than 500ms
    List<Product> GetAllProducts();

    /// <summary>
    /// Gets a product by its ID
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>Product with the specified ID, or null if not found</returns>
    [Cache(5)] // Cache for 5 minutes
    [Log]
    [Performance(200)] // Warn if takes more than 200ms
    Product? GetProductById(int id);

    /// <summary>
    /// Creates a new product
    /// </summary>
    /// <param name="product">Product to create</param>
    /// <returns>Created product</returns>
    [Validate]
    [Log]
    [Performance(1000)] // Warn if takes more than 1 second
    Product CreateProduct(Product product);

    /// <summary>
    /// Updates an existing product
    /// </summary>
    /// <param name="id">ID of the product to update</param>
    /// <param name="product">Updated product data</param>
    /// <returns>Updated product</returns>
    [Validate]
    [Log]
    [Performance(1000)] // Warn if takes more than 1 second
    [InvalidateCache("IProductService.GetProductById({id})")]
    Product UpdateProduct(int id, Product product);

    /// <summary>
    /// Deletes a product
    /// </summary>
    /// <param name="id">ID of the product to delete</param>
    [Log]
    [Performance(500)] // Warn if takes more than 500ms
    [InvalidateCache("IProductService.GetProductById({id})")]
    void DeleteProduct(int id);

    /// <summary>
    /// Gets products by category
    /// </summary>
    /// <param name="category">Category to filter by</param>
    /// <returns>List of products in the specified category</returns>
    [Cache(5)] // Cache for 5 minutes
    [Log]
    [Performance(300)] // Warn if takes more than 300ms
    List<Product> GetProductsByCategory(string category);

    /// <summary>
    /// Seeds the product data with sample products
    /// </summary>
    void SeedData();
}