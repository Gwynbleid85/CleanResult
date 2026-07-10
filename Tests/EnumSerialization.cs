using System.Text.Json.Serialization;
using CleanResult;
using Microsoft.Extensions.DependencyInjection;
using Tests.Utils;

namespace Tests;

public class EnumSerialization
{
    private enum SampleStatus
    {
        Unknown,
        Active
    }

    private class EnumDto
    {
        public SampleStatus Status { get; set; }
    }

    // The application's configured System.Text.Json options (e.g. a JsonStringEnumConverter registered via
    // ConfigureHttpJsonOptions) must be honored by Result<T>.ExecuteAsync so enums serialize as strings globally.
    [Fact]
    public async Task SuccessValueHonorsConfiguredEnumConverter()
    {
        var services = new ServiceCollection();
        services.ConfigureHttpJsonOptions(options =>
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        var serviceProvider = services.BuildServiceProvider();

        var httpContext = HttpContextUtils.GetHttpContext();
        httpContext.RequestServices = serviceProvider;

        var result = Result.Ok(new EnumDto { Status = SampleStatus.Active });
        await result.ExecuteAsync(httpContext);

        var body = HttpContextUtils.ReadContextBody(httpContext);
        Assert.Equal("""{"status":"Active"}""", body);
    }

    // Without a request service provider the serializer falls back to the camelCase default (enums as numbers).
    [Fact]
    public async Task SuccessValueFallsBackToCamelCaseWhenNoServices()
    {
        var httpContext = HttpContextUtils.GetHttpContext();

        var result = Result.Ok(new EnumDto { Status = SampleStatus.Active });
        await result.ExecuteAsync(httpContext);

        var body = HttpContextUtils.ReadContextBody(httpContext);
        Assert.Equal("""{"status":1}""", body);
    }
}
