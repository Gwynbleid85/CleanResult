using CleanResult;
using CleanResult.WolverineFx;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wolverine;
using Wolverine.Middleware;
using WolverineTests.Handlers;

namespace WolverineTests;

public class TupleResultTests
{
    private async Task<IHost> CreateHost()
    {
        return await Host.CreateDefaultBuilder()
            .UseWolverine(opts =>
            {
                opts.CodeGeneration.AddContinuationStrategy<CleanResultContinuationStrategy>();
                opts.Discovery.IncludeAssembly(typeof(TupleResultHandler).Assembly);
            })
            .StartAsync();
    }

    [Fact]
    public async Task TupleResult_Success_ShouldExtractTupleAndExecuteHandle()
    {
        // Arrange
        using var host = await CreateHost();
        var bus = host.Services.GetRequiredService<IMessageBus>();
        var command = new TupleCommand(1); // Both User and Product with ID 1 exist

        // Act
        var result = await bus.InvokeAsync<Result<OrderDto>>(command);

        // Assert
        Assert.True(result.IsOk());
        Assert.Equal(1, result.Value.UserId);
        Assert.Equal(1, result.Value.ProductId);
        Assert.Equal(1, result.Value.Quantity);
        Assert.True(result.Value.Id > 0); // Random order ID
    }

    [Fact]
    public async Task TupleResult_UserNotFound_ShouldReturnError()
    {
        // Arrange
        using var host = await CreateHost();
        var bus = host.Services.GetRequiredService<IMessageBus>();
        var command = new TupleCommand(999); // User with ID 999 doesn't exist

        // Act
        var result = await bus.InvokeAsync<Result<OrderDto>>(command);

        // Assert
        Assert.True(result.IsError());
        Assert.Equal("User not found", result.ErrorValue.Title);
        Assert.Equal(404, result.ErrorValue.Status);
    }

    [Fact]
    public async Task TupleResult_ProductNotFound_WhenUserExists_ShouldReturnError()
    {
        // Arrange
        using var host = await CreateHost();
        var bus = host.Services.GetRequiredService<IMessageBus>();

        // Add a user with ID 3 but no corresponding product
        DataStore.Users.Add(new User { Id = 3, Name = "Test User", Email = "test@example.com" });

        var command = new TupleCommand(3);

        // Act
        var result = await bus.InvokeAsync<Result<OrderDto>>(command);

        // Assert
        Assert.True(result.IsError());
        Assert.Equal("Product not found", result.ErrorValue.Title);
        Assert.Equal(404, result.ErrorValue.Status);

        // Cleanup
        DataStore.Users.RemoveAll(u => u.Id == 3);
    }

    [Fact]
    public async Task TupleResult_ValidScenario_ShouldCreateOrder()
    {
        // Arrange
        using var host = await CreateHost();
        var bus = host.Services.GetRequiredService<IMessageBus>();
        var command = new TupleCommand(2); // User and Product with ID 2 exist

        // Act
        var result = await bus.InvokeAsync<Result<OrderDto>>(command);

        // Assert
        Assert.True(result.IsOk());
        Assert.Equal(2, result.Value.UserId);
        Assert.Equal(2, result.Value.ProductId);

        // Verify we can access the order details
        var order = result.Value;
        Assert.NotNull(order);
        Assert.True(order.Id >= 1000 && order.Id <= 9999); // Random ID range
    }
}

public class AlternativeTupleResultTests
{
    private async Task<IHost> CreateHost()
    {
        return await Host.CreateDefaultBuilder()
            .UseWolverine(opts =>
            {
                opts.CodeGeneration.AddContinuationStrategy<CleanResultContinuationStrategy>();
                opts.Discovery.IncludeAssembly(typeof(AlternativeTupleHandler).Assembly);
            })
            .StartAsync();
    }

    [Fact]
    public async Task AlternativeTupleResult_Success_ShouldExtractMultipleValues()
    {
        // Arrange
        using var host = await CreateHost();
        var bus = host.Services.GetRequiredService<IMessageBus>();
        var command = new TupleCommand(4); // Even number for isValid = true

        // Act
        var result = await bus.InvokeAsync<Result<string>>(command);

        // Assert
        Assert.True(result.IsOk());
        var expectedResult = "Count: 8, Message: Message for 4, Valid: True";
        Assert.Equal(expectedResult, result.Value);
    }

    [Fact]
    public async Task AlternativeTupleResult_OddNumber_ShouldHandleCorrectly()
    {
        // Arrange
        using var host = await CreateHost();
        var bus = host.Services.GetRequiredService<IMessageBus>();
        var command = new TupleCommand(3); // Odd number for isValid = false

        // Act
        var result = await bus.InvokeAsync<Result<string>>(command);

        // Assert
        Assert.True(result.IsOk());
        var expectedResult = "Count: 6, Message: Message for 3, Valid: False";
        Assert.Equal(expectedResult, result.Value);
    }

    [Fact]
    public async Task AlternativeTupleResult_InvalidId_ShouldReturnError()
    {
        // Arrange
        using var host = await CreateHost();
        var bus = host.Services.GetRequiredService<IMessageBus>();
        var command = new TupleCommand(0); // Invalid ID

        // Act
        var result = await bus.InvokeAsync<Result<string>>(command);

        // Assert
        Assert.True(result.IsError());
        Assert.Equal("Invalid command ID", result.ErrorValue.Title);
        Assert.Equal(400, result.ErrorValue.Status);
    }
}