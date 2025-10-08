using System.ComponentModel.DataAnnotations;

namespace AspectOrientedProgramming.API.Models;

/// <summary>
/// Represents an order in the system
/// </summary>
public class Order
{
    /// <summary>
    /// Unique identifier for the order
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Customer name
    /// </summary>
    [Required(ErrorMessage = "Customer name is required")]
    [StringLength(100, ErrorMessage = "Customer name cannot exceed 100 characters")]
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Customer email
    /// </summary>
    [Required(ErrorMessage = "Customer email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string CustomerEmail { get; set; } = string.Empty;

    /// <summary>
    /// Order items
    /// </summary>
    public List<OrderItem> Items { get; set; } = [];

    /// <summary>
    /// Total amount of the order
    /// </summary>
    [Range(0.01, double.MaxValue, ErrorMessage = "Total amount must be greater than 0")]
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Status of the order
    /// </summary>
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    /// <summary>
    /// Timestamp when the order was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Timestamp when the order was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Represents an item in an order
/// </summary>
public class OrderItem
{
    /// <summary>
    /// Unique identifier for the order item
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Product identifier
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Quantity of the product
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; }

    /// <summary>
    /// Price per unit
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Total price for this item (Quantity * UnitPrice)
    /// </summary>
    public decimal TotalPrice { get; set; }
}

/// <summary>
/// Enum representing the status of an order
/// </summary>
public enum OrderStatus
{
    Pending,
    Processing,
    Shipped,
    Delivered,
    Cancelled
}
