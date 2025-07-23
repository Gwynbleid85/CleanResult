using CleanResult;
using CleanResult.WolverineFx;
using JasperFx.CodeGeneration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wolverine;
using Wolverine.Middleware;
using WolverineTests.Handlers;

namespace WolverineTests;

public class SimpleResultTests
{
    private async Task<IHost> CreateHost()
    {
        return await Host.CreateDefaultBuilder()
            .UseWolverine(opts =>
            {
                opts.CodeGeneration.AddContinuationStrategy<CleanResultContinuationStrategy>();
                opts.CodeGeneration.TypeLoadMode = TypeLoadMode.Auto;
                opts.Discovery.IncludeAssembly(typeof(SimpleResultHandler).Assembly);
            })
            .StartAsync();
    }

    [Fact]
    public async Task SimpleResult_Success_ShouldExecuteHandle()
    {
        // Arrange
        using var host = await CreateHost();
        using var scope = host.Services.CreateScope();
        var bus = scope.ServiceProvider.GetRequiredService<IMessageBus>();
        var command = new SimpleCommand(1);

        // Act
        var result = await bus.InvokeAsync<Result<string>>(command);

        // Assert
        Assert.True(result.IsOk());
        Assert.Equal("Processed command with ID: 1", result.Value);
    }

    [Fact]
    public async Task SimpleResult_LoadFails_ShouldReturnError()
    {
        // Arrange
        using var host = await CreateHost();
        using var scope = host.Services.CreateScope();
        var bus = scope.ServiceProvider.GetRequiredService<IMessageBus>();
        var command = new SimpleCommand(-1); // This will fail in LoadAsync

        // Act
        var result = await bus.InvokeAsync<Result<string>>(command);

        // Assert
        Assert.True(result.IsError());
        Assert.Equal("Invalid ID", result.ErrorValue.Title);
        Assert.Equal(400, result.ErrorValue.Status);
    }

    [Fact]
    public async Task SimpleResult_NotFound_ShouldReturnNotFoundError()
    {
        // Arrange
        using var host = await CreateHost();
        using var scope = host.Services.CreateScope();
        var bus = scope.ServiceProvider.GetRequiredService<IMessageBus>();
        var command = new SimpleCommand(999); // This will return not found in LoadAsync

        // Act
        var result = await bus.InvokeAsync<Result<string>>(command);

        // Assert
        Assert.True(result.IsError());
        Assert.Equal("Entity not found", result.ErrorValue.Title);
        Assert.Equal(404, result.ErrorValue.Status);
    }
}