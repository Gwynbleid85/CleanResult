using CleanResult;
using CleanResult.WolverineFx;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wolverine;
using Wolverine.Middleware;
using WolverineTests.Handlers;

namespace WolverineTests;

public class GenericResultTests
{
    private async Task<IHost> CreateHost()
    {
        return await Host.CreateDefaultBuilder()
            .UseWolverine(opts =>
            {
                opts.CodeGeneration.AddContinuationStrategy<CleanResultContinuationStrategy>();
                opts.Discovery.IncludeAssembly(typeof(GenericResultHandler).Assembly);
            })
            .StartAsync();
    }

    [Fact]
    public async Task GenericResult_Success_ShouldExtractValueAndExecuteHandle()
    {
        // Arrange
        using var host = await CreateHost();
        var bus = host.Services.GetRequiredService<IMessageBus>();
        var command = new GenericCommand(1); // User with ID 1 exists in DataStore

        // Act
        var result = await bus.InvokeAsync<Result<UserDto>>(command);

        // Assert
        Assert.True(result.IsOk());
        Assert.Equal(1, result.Value.Id);
        Assert.Equal("John Doe", result.Value.Name);
        Assert.Equal("john@example.com", result.Value.Email);
    }

    [Fact]
    public async Task GenericResult_LoadFails_ShouldReturnError()
    {
        // Arrange
        using var host = await CreateHost();
        var bus = host.Services.GetRequiredService<IMessageBus>();
        var command = new GenericCommand(999); // User with ID 999 doesn't exist

        // Act
        var result = await bus.InvokeAsync<Result<UserDto>>(command);

        // Assert
        Assert.True(result.IsError());
        Assert.Equal("User not found", result.ErrorValue.Title);
        Assert.Equal(404, result.ErrorValue.Status);
    }

    [Fact]
    public async Task GenericResult_MultipleUsers_ShouldHandleCorrectly()
    {
        // Arrange
        using var host = await CreateHost();
        var bus = host.Services.GetRequiredService<IMessageBus>();

        // Act & Assert for User 1
        var result1 = await bus.InvokeAsync<Result<UserDto>>(new GenericCommand(1));
        Assert.True(result1.IsOk());
        Assert.Equal("John Doe", result1.Value.Name);

        // Act & Assert for User 2
        var result2 = await bus.InvokeAsync<Result<UserDto>>(new GenericCommand(2));
        Assert.True(result2.IsOk());
        Assert.Equal("Jane Smith", result2.Value.Name);
    }
}