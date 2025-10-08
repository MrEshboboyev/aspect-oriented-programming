using AspectOrientedProgramming.API.Attributes;
using AspectOrientedProgramming.API.Models;
using AspectOrientedProgramming.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace AspectOrientedProgramming.API.Controllers;

[ApiController]
[Route("[controller]")]
public class OrdersController(
    IOrderService orderService,
    ILogger<OrdersController> logger
) : ControllerBase
{
    /// <summary>
    /// Gets all orders
    /// </summary>
    /// <returns>List of all orders</returns>
    [HttpGet]
    [Log]
    [Performance(500)]
    public IActionResult GetAllOrders()
    {
        logger.LogDebug("GET /orders endpoint called");
        var orders = orderService.GetAllOrders();
        logger.LogInformation("Returning {OrderCount} orders", orders.Count);
        return Ok(orders);
    }

    /// <summary>
    /// Gets an order by its ID
    /// </summary>
    /// <param name="id">Order ID</param>
    /// <returns>Order with the specified ID</returns>
    [HttpGet("{id}")]
    [Log]
    [Performance(200)]
    public IActionResult GetOrderById(int id)
    {
        logger.LogDebug("GET /orders/{id} endpoint called with ID: {OrderId}", id, id);
        var order = orderService.GetOrderById(id);
        if (order == null)
        {
            logger.LogWarning("Order with ID {OrderId} not found", id);
            return NotFound();
        }
        logger.LogInformation("Returning order for customer: {CustomerName} (ID: {OrderId})", order.CustomerName, order.Id);
        return Ok(order);
    }

    /// <summary>
    /// Creates a new order
    /// </summary>
    /// <param name="order">Order to create</param>
    /// <returns>Created order</returns>
    [HttpPost]
    [Validate]
    [Log]
    [Performance(1000)]
    public IActionResult CreateOrder(Order order)
    {
        logger.LogInformation("POST /orders endpoint called to create order for customer: {CustomerName}", order.CustomerName);
        var createdOrder = orderService.CreateOrder(order);
        logger.LogInformation("Order created successfully with ID: {OrderId}", createdOrder.Id);
        return CreatedAtAction(nameof(GetOrderById), new { id = createdOrder.Id }, createdOrder);
    }

    /// <summary>
    /// Updates an existing order
    /// </summary>
    /// <param name="id">ID of the order to update</param>
    /// <param name="order">Updated order data</param>
    /// <returns>Updated order</returns>
    [HttpPut("{id}")]
    [Validate]
    [Log]
    [Performance(1000)]
    public IActionResult UpdateOrder(int id, Order order)
    {
        logger.LogInformation("PUT /orders/{id} endpoint called to update order ID: {OrderId}", id, id);
        try
        {
            var updatedOrder = orderService.UpdateOrder(id, order);
            logger.LogInformation("Order updated successfully for customer: {CustomerName} (ID: {OrderId})", updatedOrder.CustomerName, updatedOrder.Id);
            return Ok(updatedOrder);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning("Failed to update order ID {OrderId}: {ErrorMessage}", id, ex.Message);
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Deletes an order
    /// </summary>
    /// <param name="id">ID of the order to delete</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    [Log]
    [Performance(500)]
    public IActionResult DeleteOrder(int id)
    {
        logger.LogInformation("DELETE /orders/{id} endpoint called to delete order ID: {OrderId}", id, id);
        orderService.DeleteOrder(id);
        logger.LogInformation("Order ID {OrderId} deleted successfully", id);
        return NoContent();
    }

    /// <summary>
    /// Gets orders by status
    /// </summary>
    /// <param name="status">Status to filter by</param>
    /// <returns>List of orders with the specified status</returns>
    [HttpGet("status/{status}")]
    [Log]
    [Performance(300)]
    public IActionResult GetOrdersByStatus(OrderStatus status)
    {
        logger.LogDebug("GET /orders/status/{status} endpoint called with status: {OrderStatus}", status, status);
        var orders = orderService.GetOrdersByStatus(status);
        logger.LogInformation("Returning {OrderCount} orders with status: {OrderStatus}", orders.Count, status);
        return Ok(orders);
    }
}