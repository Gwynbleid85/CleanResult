using CleanResult;
using CleanResult.WolverineFx;
using JasperFx.CodeGeneration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wolverine;
using Wolverine.Middleware;
using WolverineTests.Handlers;

namespace WolverineTests;

public class InnerClassTests
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
    public async Task InnerClass_Success_ShouldExecuteHandle()
    {
        // Arrange
        using var host = await CreateHost();
        using var scope = host.Services.CreateScope();
        var bus = scope.ServiceProvider.GetRequiredService<IMessageBus>();
        var command = new InnerCommand(1);

        // Act
        var result = await bus.InvokeAsync<Result<User>>(command);

        // Assert
        Assert.True(result.IsOk());
        Assert.Equal(42, result.Value.Id);
    }
}