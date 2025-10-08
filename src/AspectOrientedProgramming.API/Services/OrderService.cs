using AspectOrientedProgramming.API.Models;

namespace AspectOrientedProgramming.API.Services;

/// <summary>
/// Implementation of order service operations with detailed logging
/// </summary>
public class OrderService(
    ILogger<OrderService> logger
) : IOrderService
{
    private static readonly List<Order> _orders = new List<Order>();
    private static int _nextId = 1;

    /// <inheritdoc />
    public List<Order> GetAllOrders()
    {
        logger.LogDebug("GetAllOrders method called");
        
        // Simulate some processing time
        Thread.Sleep(100);
        logger.LogInformation("Retrieved {OrderCount} orders from storage", _orders.Count);
        return [.. _orders];
    }

    /// <inheritdoc />
    public Order? GetOrderById(int id)
    {
        logger.LogDebug("GetOrderById method called with ID: {OrderId}", id);
        
        // Simulate some processing time
        Thread.Sleep(50);
        var order = _orders.FirstOrDefault(o => o.Id == id);
        if (order != null)
        {
            logger.LogInformation("Found order with ID: {OrderId}, Customer: {CustomerName}", id, order.CustomerName);
        }
        else
        {
            logger.LogWarning("Order with ID: {OrderId} not found", id);
        }
        return order;
    }

    /// <inheritdoc />
    public Order CreateOrder(Order order)
    {
        logger.LogInformation("Creating new order for customer: {CustomerName}", order.CustomerName);
        
        order.Id = _nextId++;
        order.CreatedAt = DateTime.UtcNow;
        order.UpdatedAt = DateTime.UtcNow;
        
        // Calculate total amount
        order.TotalAmount = order.Items.Sum(item => item.TotalPrice);
        
        _orders.Add(order);
        
        logger.LogInformation("Order created successfully with ID {OrderId} for customer {CustomerName}, Total Amount: {TotalAmount:C}", 
            order.Id, order.CustomerName, order.TotalAmount);
        return order;
    }

    /// <inheritdoc />
    public Order UpdateOrder(int id, Order order)
    {
        logger.LogInformation("Updating order with ID: {OrderId}", id);
        
        var existingOrder = _orders.FirstOrDefault(o => o.Id == id) 
            ?? throw new ArgumentException($"Order with ID {id} not found");
        
        logger.LogDebug("Found existing order: {OrderId} for customer {CustomerName}", existingOrder.Id, existingOrder.CustomerName);
        
        existingOrder.CustomerName = order.CustomerName;
        existingOrder.CustomerEmail = order.CustomerEmail;
        existingOrder.Items = order.Items;
        existingOrder.Status = order.Status;
        existingOrder.UpdatedAt = DateTime.UtcNow;
        
        // Recalculate total amount
        existingOrder.TotalAmount = existingOrder.Items.Sum(item => item.TotalPrice);
        
        logger.LogInformation("Order {OrderId} updated successfully for customer {CustomerName}, New Total Amount: {TotalAmount:C}", 
            id, order.CustomerName, existingOrder.TotalAmount);
        return existingOrder;
    }

    /// <inheritdoc />
    public void DeleteOrder(int id)
    {
        logger.LogInformation("Deleting order with ID: {OrderId}", id);
        
        var order = _orders.FirstOrDefault(o => o.Id == id);
        if (order != null)
        {
            _orders.Remove(order);
            logger.LogInformation("Order {OrderId} for customer {CustomerName} deleted successfully", order.Id, order.CustomerName);
        }
        else
        {
            logger.LogWarning("Attempted to delete non-existent order with ID: {OrderId}", id);
        }
    }

    /// <inheritdoc />
    public List<Order> GetOrdersByStatus(OrderStatus status)
    {
        logger.LogDebug("GetOrdersByStatus method called with status: {OrderStatus}", status);
        
        // Simulate some processing time
        Thread.Sleep(75);
        var filteredOrders = new List<Order>(_orders.Where(o => o.Status == status));
        logger.LogInformation("Found {OrderCount} orders with status: {OrderStatus}", filteredOrders.Count, status);
        return filteredOrders;
    }

    /// <summary>
    /// Seeds the order data with sample orders
    /// </summary>
    public void SeedData()
    {
        if (_orders.Count == 0)
        {
            logger.LogInformation("Seeding initial order data");
            
            _orders.AddRange(new List<Order>
            {
                new Order 
                { 
                    Id = _nextId++, 
                    CustomerName = "John Doe", 
                    CustomerEmail = "john.doe@example.com", 
                    Status = OrderStatus.Processing,
                    Items = new List<OrderItem>
                    {
                        new OrderItem { Id = 1, ProductId = 1, Quantity = 1, UnitPrice = 1200.00m, TotalPrice = 1200.00m }
                    },
                    TotalAmount = 1200.00m,
                    CreatedAt = DateTime.UtcNow, 
                    UpdatedAt = DateTime.UtcNow 
                },
                new Order 
                { 
                    Id = _nextId++, 
                    CustomerName = "Jane Smith", 
                    CustomerEmail = "jane.smith@example.com", 
                    Status = OrderStatus.Shipped,
                    Items = new List<OrderItem>
                    {
                        new OrderItem { Id = 1, ProductId = 2, Quantity = 2, UnitPrice = 800.00m, TotalPrice = 1600.00m },
                        new OrderItem { Id = 2, ProductId = 4, Quantity = 1, UnitPrice = 120.00m, TotalPrice = 120.00m }
                    },
                    TotalAmount = 1720.00m,
                    CreatedAt = DateTime.UtcNow, 
                    UpdatedAt = DateTime.UtcNow 
                }
            });
            
            logger.LogInformation("Seeded {OrderCount} sample orders", _orders.Count);
        }
        else
        {
            logger.LogDebug("Order data already seeded, skipping");
        }
    }
}
