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
    List<Product> GetAllProducts();

    /// <summary>
    /// Gets a product by its ID
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>Product with the specified ID, or null if not found</returns>
    Product? GetProductById(int id);

    /// <summary>
    /// Creates a new product
    /// </summary>
    /// <param name="product">Product to create</param>
    /// <returns>Created product</returns>
    Product CreateProduct(Product product);

    /// <summary>
    /// Updates an existing product
    /// </summary>
    /// <param name="id">ID of the product to update</param>
    /// <param name="product">Updated product data</param>
    /// <returns>Updated product</returns>
    Product UpdateProduct(int id, Product product);

    /// <summary>
    /// Deletes a product
    /// </summary>
    /// <param name="id">ID of the product to delete</param>
    void DeleteProduct(int id);

    /// <summary>
    /// Gets products by category
    /// </summary>
    /// <param name="category">Category to filter by</param>
    /// <returns>List of products in the specified category</returns>
    List<Product> GetProductsByCategory(string category);
}
