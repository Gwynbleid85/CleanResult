using CleanResult;
using CleanResult.WolverineFx;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wolverine;
using Wolverine.Middleware;
using WolverineTests.Handlers;

namespace WolverineTests;

public class CascadingTupleResultTests
{
    [Fact]
    public async Task CascadingTupleResult_Error_ShouldReturnTupleWithErrorAndNullFollowers()
    {
        using var host = await Host.CreateDefaultBuilder()
            .UseWolverine(opts =>
            {
                opts.CodeGeneration.AddContinuationStrategy<CleanResultContinuationStrategy>();
                opts.Discovery.IncludeAssembly(typeof(CascadingTupleHandler).Assembly);
            })
            .StartAsync();

        var bus = host.Services.GetRequiredService<IMessageBus>();

        var result = await bus.InvokeAsync<(Result<int> result, string? message, int? value)>(new CascadingTupleCommand(0));

        Assert.True(result.result.IsError());
        Assert.Equal("Invalid command ID", result.result.ErrorValue.Title);
        Assert.Null(result.message);
        Assert.Null(result.value);
    }
}
