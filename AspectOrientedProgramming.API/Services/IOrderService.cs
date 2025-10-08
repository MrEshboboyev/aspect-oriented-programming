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
    List<Order> GetAllOrders();

    /// <summary>
    /// Gets an order by its ID
    /// </summary>
    /// <param name="id">Order ID</param>
    /// <returns>Order with the specified ID, or null if not found</returns>
    Order? GetOrderById(int id);

    /// <summary>
    /// Creates a new order
    /// </summary>
    /// <param name="order">Order to create</param>
    /// <returns>Created order</returns>
    Order CreateOrder(Order order);

    /// <summary>
    /// Updates an existing order
    /// </summary>
    /// <param name="id">ID of the order to update</param>
    /// <param name="order">Updated order data</param>
    /// <returns>Updated order</returns>
    Order UpdateOrder(int id, Order order);

    /// <summary>
    /// Deletes an order
    /// </summary>
    /// <param name="id">ID of the order to delete</param>
    void DeleteOrder(int id);

    /// <summary>
    /// Gets orders by status
    /// </summary>
    /// <param name="status">Status to filter by</param>
    /// <returns>List of orders with the specified status</returns>
    List<Order> GetOrdersByStatus(OrderStatus status);
}
