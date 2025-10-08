using AspectOrientedProgramming.API.Attributes;
using AspectOrientedProgramming.API.Models;
using AspectOrientedProgramming.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace AspectOrientedProgramming.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductsController(
    IProductService productService,
    ILogger<ProductsController> logger
) : ControllerBase
{
    /// <summary>
    /// Gets all products
    /// </summary>
    /// <returns>List of all products</returns>
    [HttpGet]
    [Log]
    [Performance(500)]
    public IActionResult GetAllProducts()
    {
        logger.LogDebug("GET /products endpoint called");
        var products = productService.GetAllProducts();
        logger.LogInformation("Returning {ProductCount} products", products.Count);
        return Ok(products);
    }

    /// <summary>
    /// Gets a product by its ID
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>Product with the specified ID</returns>
    [HttpGet("{id}")]
    [Log]
    [Performance(200)]
    public IActionResult GetProductById(int id)
    {
        logger.LogDebug("GET /products/{id} endpoint called with ID: {ProductId}", id, id);
        var product = productService.GetProductById(id);
        if (product == null)
        {
            logger.LogWarning("Product with ID {ProductId} not found", id);
            return NotFound();
        }
        logger.LogInformation("Returning product: {ProductName} (ID: {ProductId})", product.Name, product.Id);
        return Ok(product);
    }

    /// <summary>
    /// Creates a new product
    /// </summary>
    /// <param name="product">Product to create</param>
    /// <returns>Created product</returns>
    [HttpPost]
    [Validate]
    [Log]
    [Performance(1000)]
    public IActionResult CreateProduct(Product product)
    {
        logger.LogInformation("POST /products endpoint called to create product: {ProductName}", product.Name);
        var createdProduct = productService.CreateProduct(product);
        logger.LogInformation("Product created successfully with ID: {ProductId}", createdProduct.Id);
        return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.Id }, createdProduct);
    }

    /// <summary>
    /// Updates an existing product
    /// </summary>
    /// <param name="id">ID of the product to update</param>
    /// <param name="product">Updated product data</param>
    /// <returns>Updated product</returns>
    [HttpPut("{id}")]
    [Validate]
    [Log]
    [Performance(1000)]
    public IActionResult UpdateProduct(int id, Product product)
    {
        logger.LogInformation("PUT /products/{id} endpoint called to update product ID: {ProductId}", id, id);
        try
        {
            var updatedProduct = productService.UpdateProduct(id, product);
            logger.LogInformation("Product updated successfully: {ProductName} (ID: {ProductId})", updatedProduct.Name, updatedProduct.Id);
            return Ok(updatedProduct);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning("Failed to update product ID {ProductId}: {ErrorMessage}", id, ex.Message);
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Deletes a product
    /// </summary>
    /// <param name="id">ID of the product to delete</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    [Log]
    [Performance(500)]
    public IActionResult DeleteProduct(int id)
    {
        logger.LogInformation("DELETE /products/{id} endpoint called to delete product ID: {ProductId}", id, id);
        productService.DeleteProduct(id);
        logger.LogInformation("Product ID {ProductId} deleted successfully", id);
        return NoContent();
    }

    /// <summary>
    /// Gets products by category
    /// </summary>
    /// <param name="category">Category to filter by</param>
    /// <returns>List of products in the specified category</returns>
    [HttpGet("category/{category}")]
    [Log]
    [Performance(300)]
    public IActionResult GetProductsByCategory(string category)
    {
        logger.LogDebug("GET /products/category/{category} endpoint called with category: {Category}", category, category);
        var products = productService.GetProductsByCategory(category);
        logger.LogInformation("Returning {ProductCount} products in category: {Category}", products.Count, category);
        return Ok(products);
    }
}