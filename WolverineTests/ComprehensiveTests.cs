using CleanResult;
using CleanResult.WolverineFx;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wolverine;
using Wolverine.Middleware;
using WolverineTests.Handlers;

namespace WolverineTests;

/// <summary>
/// Comprehensive tests that demonstrate all CleanResult continuation strategy scenarios
/// </summary>
public class ComprehensiveTests
{
    private async Task<IHost> CreateHost()
    {
        return await Host.CreateDefaultBuilder()
            .UseWolverine(opts =>
            {
                opts.CodeGeneration.AddContinuationStrategy<CleanResultContinuationStrategy>();
                opts.Discovery.IncludeAssembly(typeof(SimpleResultHandler).Assembly);
            })
            .StartAsync();
    }

    [Fact]
    public async Task AllScenarios_Success_ShouldWork()
    {
        // Arrange
        using var host = await CreateHost();
        using var scope = host.Services.CreateScope();
        var bus = scope.ServiceProvider.GetRequiredService<IMessageBus>();

        // Act & Assert - Simple Result
        var simpleResult = await bus.InvokeAsync<Result<string>>(new SimpleCommand(1));
        Assert.True(simpleResult.IsOk());
        Assert.Equal("Processed command with ID: 1", simpleResult.Value);

        // Act & Assert - Generic Result
        var genericResult = await bus.InvokeAsync<Result<UserDto>>(new GenericCommand(1));
        Assert.True(genericResult.IsOk());
        Assert.Equal("John Doe", genericResult.Value.Name);

        // Act & Assert - Tuple Result
        var tupleResult = await bus.InvokeAsync<Result<OrderDto>>(new TupleCommand(1));
        Assert.True(tupleResult.IsOk());
        Assert.Equal(1, tupleResult.Value.UserId);
        Assert.Equal(1, tupleResult.Value.ProductId);

        // Act & Assert - Alternative Tuple Result
        var altTupleResult = await bus.InvokeAsync<Result<string>>(new AlternativeTupleCommand(4));
        Assert.True(altTupleResult.IsOk());
        Assert.Contains("Count: 8", altTupleResult.Value);
        Assert.Contains("Valid: True", altTupleResult.Value);
    }

    [Fact]
    public async Task AllScenarios_Errors_ShouldReturnAppropriateErrors()
    {
        // Arrange
        using var host = await CreateHost();
        using var scope = host.Services.CreateScope();
        var bus = scope.ServiceProvider.GetRequiredService<IMessageBus>();

        // Act & Assert - Simple Result Error
        var simpleError = await bus.InvokeAsync<Result<string>>(new SimpleCommand(-1));
        Assert.True(simpleError.IsError());
        Assert.Equal("Invalid ID", simpleError.ErrorValue.Title);
        Assert.Equal(400, simpleError.ErrorValue.Status);

        // Act & Assert - Generic Result Error
        var genericError = await bus.InvokeAsync<Result<UserDto>>(new GenericCommand(999));
        Assert.True(genericError.IsError());
        Assert.Equal("User not found", genericError.ErrorValue.Title);
        Assert.Equal(404, genericError.ErrorValue.Status);

        // Act & Assert - Tuple Result Error
        var tupleError = await bus.InvokeAsync<Result<OrderDto>>(new TupleCommand(999));
        Assert.True(tupleError.IsError());
        Assert.Equal("User not found", tupleError.ErrorValue.Title);
        Assert.Equal(404, tupleError.ErrorValue.Status);

        // Act & Assert - Alternative Tuple Result Error
        var altTupleError = await bus.InvokeAsync<Result<string>>(new AlternativeTupleCommand(0));
        Assert.True(altTupleError.IsError());
        Assert.Equal("Invalid command ID", altTupleError.ErrorValue.Title);
        Assert.Equal(400, altTupleError.ErrorValue.Status);
    }

    [Theory]
    [InlineData(1, "John Doe")]
    [InlineData(2, "Jane Smith")]
    public async Task ParameterizedTest_GenericResults_ShouldWorkWithDifferentIds(int userId, string expectedName)
    {
        // Arrange
        using var host = await CreateHost();
        using var scope = host.Services.CreateScope();
        var bus = scope.ServiceProvider.GetRequiredService<IMessageBus>();

        // Act
        var result = await bus.InvokeAsync<Result<UserDto>>(new GenericCommand(userId));

        // Assert
        Assert.True(result.IsOk());
        Assert.Equal(expectedName, result.Value.Name);
        Assert.Equal(userId, result.Value.Id);
    }

    [Theory]
    [InlineData(2, 4, "Message for 2", true)]  // Even number
    [InlineData(3, 6, "Message for 3", false)] // Odd number
    [InlineData(5, 10, "Message for 5", false)] // Odd number
    public async Task ParameterizedTest_TupleResults_ShouldHandleDifferentTupleValues(
        int commandId, int expectedCount, string expectedMessage, bool expectedValid)
    {
        // Arrange
        using var host = await CreateHost();
        using var scope = host.Services.CreateScope();
        var bus = scope.ServiceProvider.GetRequiredService<IMessageBus>();

        // Act
        var result = await bus.InvokeAsync<Result<string>>(new AlternativeTupleCommand(commandId));

        // Assert
        Assert.True(result.IsOk());
        Assert.Contains($"Count: {expectedCount}", result.Value);
        Assert.Contains($"Message: {expectedMessage}", result.Value);
        Assert.Contains($"Valid: {expectedValid}", result.Value);
    }
}