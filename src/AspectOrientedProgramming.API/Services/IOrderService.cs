using AspectOrientedProgramming.API.Attributes;
using AspectOrientedProgramming.API.Models;

namespace AspectOrientedProgramming.API.Services;

/// <summary>
/// Interface for order service operations
/// </summary>
public interface IOrderService
{
    /// <summary>
    /// Gets all orders
    /// </summary>
    /// <returns>List of all orders</returns>
    [Cache(10)] // Cache for 10 minutes
    [Log]
    [Performance(500)] // Warn if takes more than 500ms
    List<Order> GetAllOrders();

    /// <summary>
    /// Gets an order by its ID
    /// </summary>
    /// <param name="id">Order ID</param>
    /// <returns>Order with the specified ID, or null if not found</returns>
    [Cache(5)] // Cache for 5 minutes
    [Log]
    [Performance(200)] // Warn if takes more than 200ms
    Order? GetOrderById(int id);

    /// <summary>
    /// Creates a new order
    /// </summary>
    /// <param name="order">Order to create</param>
    /// <returns>Created order</returns>
    [Validate]
    [Log]
    [Performance(1000)] // Warn if takes more than 1 second
    Order CreateOrder(Order order);

    /// <summary>
    /// Updates an existing order
    /// </summary>
    /// <param name="id">ID of the order to update</param>
    /// <param name="order">Updated order data</param>
    /// <returns>Updated order</returns>
    [Validate]
    [Log]
    [Performance(1000)] // Warn if takes more than 1 second
    [InvalidateCache("IOrderService.GetOrderById({id})")]
    Order UpdateOrder(int id, Order order);

    /// <summary>
    /// Deletes an order
    /// </summary>
    /// <param name="id">ID of the order to delete</param>
    [Log]
    [Performance(500)] // Warn if takes more than 500ms
    [InvalidateCache("IOrderService.GetOrderById({id})")]
    void DeleteOrder(int id);

    /// <summary>
    /// Gets orders by status
    /// </summary>
    /// <param name="status">Status to filter by</param>
    /// <returns>List of orders with the specified status</returns>
    [Cache(5)] // Cache for 5 minutes
    [Log]
    [Performance(300)] // Warn if takes more than 300ms
    List<Order> GetOrdersByStatus(OrderStatus status);

    /// <summary>
    /// Seeds the order data with sample orders
    /// </summary>
    void SeedData();
}