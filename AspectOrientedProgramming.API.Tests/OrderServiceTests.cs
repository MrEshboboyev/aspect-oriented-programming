using AspectOrientedProgramming.API.Models;
using AspectOrientedProgramming.API.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace AspectOrientedProgramming.API.Tests;

public class OrderServiceTests
{
    private Mock<ILogger<OrderService>> _mockLogger;
    private OrderService _orderService;

    public OrderServiceTests()
    {
        _mockLogger = new Mock<ILogger<OrderService>>();
        _orderService = new OrderService(_mockLogger.Object);
    }

    [Fact]
    public void OrderService_Should_Create_Order()
    {
        // Arrange
        var order = new Order
        {
            CustomerName = "John Doe",
            CustomerEmail = "john.doe@example.com",
            Items =
            [
                new() { ProductId = 1, Quantity = 2, UnitPrice = 10.99m, TotalPrice = 21.98m }
            ],
            Status = OrderStatus.Pending
        };

        // Act
        var createdOrder = _orderService.CreateOrder(order);

        // Assert
        Assert.NotNull(createdOrder);
        Assert.Equal(order.CustomerName, createdOrder.CustomerName);
        Assert.Equal(order.CustomerEmail, createdOrder.CustomerEmail);
        Assert.Equal(order.Items.Count, createdOrder.Items.Count);
        Assert.Equal(OrderStatus.Pending, createdOrder.Status);
        Assert.True(createdOrder.Id > 0);
        Assert.True(createdOrder.TotalAmount > 0);
    }

    [Fact]
    public void OrderService_Should_Get_Order_By_Id()
    {
        // Arrange
        var order = new Order
        {
            CustomerName = "John Doe",
            CustomerEmail = "john.doe@example.com",
            Items =
            [
                new() { ProductId = 1, Quantity = 2, UnitPrice = 10.99m, TotalPrice = 21.98m }
            ],
            Status = OrderStatus.Pending
        };
        var createdOrder = _orderService.CreateOrder(order);

        // Act
        var retrievedOrder = _orderService.GetOrderById(createdOrder.Id);

        // Assert
        Assert.NotNull(retrievedOrder);
        Assert.Equal(createdOrder.Id, retrievedOrder.Id);
        Assert.Equal(createdOrder.CustomerName, retrievedOrder.CustomerName);
    }

    [Fact]
    public void OrderService_Should_Return_Null_For_NonExistent_Order()
    {
        // Act
        var order = _orderService.GetOrderById(999);

        // Assert
        Assert.Null(order);
    }

    [Fact]
    public void OrderService_Should_Update_Order()
    {
        // Arrange
        var order = new Order
        {
            CustomerName = "John Doe",
            CustomerEmail = "john.doe@example.com",
            Items =
            [
                new() { ProductId = 1, Quantity = 2, UnitPrice = 10.99m, TotalPrice = 21.98m }
            ],
            Status = OrderStatus.Pending
        };
        var createdOrder = _orderService.CreateOrder(order);

        var updatedOrder = new Order
        {
            CustomerName = "Jane Smith",
            CustomerEmail = "jane.smith@example.com",
            Items =
            [
                new() { ProductId = 1, Quantity = 3, UnitPrice = 15.99m, TotalPrice = 47.97m }
            ],
            Status = OrderStatus.Processing
        };

        // Act
        var result = _orderService.UpdateOrder(createdOrder.Id, updatedOrder);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createdOrder.Id, result.Id);
        Assert.Equal(updatedOrder.CustomerName, result.CustomerName);
        Assert.Equal(updatedOrder.CustomerEmail, result.CustomerEmail);
        Assert.Equal(updatedOrder.Items.Count, result.Items.Count);
        Assert.Equal(updatedOrder.Status, result.Status);
        Assert.Equal(47.97m, result.TotalAmount);
    }

    [Fact]
    public void OrderService_Should_Throw_Exception_For_NonExistent_Order_Update()
    {
        // Arrange
        var order = new Order
        {
            CustomerName = "John Doe",
            CustomerEmail = "john.doe@example.com",
            Items =
            [
                new() { ProductId = 1, Quantity = 2, UnitPrice = 10.99m, TotalPrice = 21.98m }
            ],
            Status = OrderStatus.Pending
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _orderService.UpdateOrder(999, order));
        Assert.Contains("not found", exception.Message);
    }

    [Fact]
    public void OrderService_Should_Delete_Order()
    {
        // Arrange
        var order = new Order
        {
            CustomerName = "John Doe",
            CustomerEmail = "john.doe@example.com",
            Items =
            [
                new() { ProductId = 1, Quantity = 2, UnitPrice = 10.99m, TotalPrice = 21.98m }
            ],
            Status = OrderStatus.Pending
        };
        var createdOrder = _orderService.CreateOrder(order);

        // Act
        _orderService.DeleteOrder(createdOrder.Id);

        // Assert
        var retrievedOrder = _orderService.GetOrderById(createdOrder.Id);
        Assert.Null(retrievedOrder);
    }

    [Fact]
    public void OrderService_Should_Get_Orders_By_Status()
    {
        // First, get the current count of pending orders
        var existingPendingOrders = _orderService.GetOrdersByStatus(OrderStatus.Pending);
        var existingCount = existingPendingOrders.Count;

        // Arrange
        var order1 = new Order
        {
            CustomerName = "John Doe",
            CustomerEmail = "john.doe@example.com",
            Items = [],
            Status = OrderStatus.Pending
        };
        var order2 = new Order
        {
            CustomerName = "Jane Smith",
            CustomerEmail = "jane.smith@example.com",
            Items = [],
            Status = OrderStatus.Processing
        };
        var order3 = new Order
        {
            CustomerName = "Bob Johnson",
            CustomerEmail = "bob.johnson@example.com",
            Items = [],
            Status = OrderStatus.Pending
        };

        _orderService.CreateOrder(order1);
        _orderService.CreateOrder(order2);
        _orderService.CreateOrder(order3);

        // Act
        var pendingOrders = _orderService.GetOrdersByStatus(OrderStatus.Pending);

        // Assert
        Assert.Equal(existingCount + 2, pendingOrders.Count);
        Assert.All(pendingOrders.Skip(existingCount), o => Assert.Equal(OrderStatus.Pending, o.Status));
    }
}
