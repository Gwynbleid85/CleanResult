using System.Text.Json;
using CleanResult;
using CleanResult.AspNet;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Tests.Utils;

namespace Tests;

public class CleanResultConfigurationTests : IDisposable
{
    public CleanResultConfigurationTests()
    {
        ResetConfiguration();
    }

    public void Dispose()
    {
        ResetConfiguration();
    }

    [Fact]
    public async Task DefaultSimpleSuccessStatusCodeHasDefaultValue()
    {
        Assert.Equal(StatusCodes.Status204NoContent, CleanResultConfiguration.Options.AspNetCore.DefaultSimpleSuccessStatusCode);

        var httpContext = HttpContextUtils.GetHttpContext();
        await Result.Ok().ExecuteAsync(httpContext);

        Assert.Equal(StatusCodes.Status204NoContent, httpContext.Response.StatusCode);
    }

    [Fact]
    public async Task DefaultSimpleSuccessStatusCodeCanBeOverridden()
    {
        CleanResultConfiguration.Options.AspNetCore.DefaultSimpleSuccessStatusCode = StatusCodes.Status202Accepted;

        var httpContext = HttpContextUtils.GetHttpContext();
        await Result.Ok().ExecuteAsync(httpContext);

        Assert.Equal(StatusCodes.Status202Accepted, httpContext.Response.StatusCode);
    }

    [Fact]
    public async Task DefaultValueSuccessStatusCodeHasDefaultValue()
    {
        Assert.Equal(StatusCodes.Status200OK, CleanResultConfiguration.Options.AspNetCore.DefaultValueSuccessStatusCode);

        var httpContext = HttpContextUtils.GetHttpContext();
        await Result.Ok(new { Message = "Success" }).ExecuteAsync(httpContext);

        Assert.Equal(StatusCodes.Status200OK, httpContext.Response.StatusCode);
    }

    [Fact]
    public async Task DefaultValueSuccessStatusCodeCanBeOverridden()
    {
        CleanResultConfiguration.Options.AspNetCore.DefaultValueSuccessStatusCode = StatusCodes.Status201Created;

        var httpContext = HttpContextUtils.GetHttpContext();
        await Result.Ok(new { Message = "Success" }).ExecuteAsync(httpContext);

        Assert.Equal(StatusCodes.Status201Created, httpContext.Response.StatusCode);
    }

    [Fact]
    public async Task JsonSerializerOptionsHasDefaultValue()
    {
        Assert.Equal(JsonNamingPolicy.CamelCase, CleanResultConfiguration.Options.AspNetCore.JsonSerializerOptions.PropertyNamingPolicy);

        var httpContext = HttpContextUtils.GetHttpContext();
        await Result.Ok(new { FirstName = "Ada" }).ExecuteAsync(httpContext);

        Assert.Equal("""{"firstName":"Ada"}""", HttpContextUtils.ReadContextBody(httpContext));
    }

    [Fact]
    public async Task JsonSerializerOptionsCanBeOverridden()
    {
        CleanResultConfiguration.Options.AspNetCore.JsonSerializerOptions = new JsonSerializerOptions();

        var httpContext = HttpContextUtils.GetHttpContext();
        await Result.Ok(new { FirstName = "Ada" }).ExecuteAsync(httpContext);

        Assert.Equal("""{"FirstName":"Ada"}""", HttpContextUtils.ReadContextBody(httpContext));
    }

    [Fact]
    public void DefaultSuccessSerializerUsesUpdatedJsonSerializerOptions()
    {
        CleanResultConfiguration.Options.AspNetCore.JsonSerializerOptions = new JsonSerializerOptions();

        var serialized = CleanResultConfiguration.Options.AspNetCore.SuccessSerializer(
            new { FirstName = "Ada" }
        );

        Assert.Equal("""{"FirstName":"Ada"}""", serialized);
    }

    [Fact]
    public void DefaultErrorStatusCodeHasDefaultValue()
    {
        Assert.Equal(StatusCodes.Status500InternalServerError, CleanResultConfiguration.Options.Errors.DefaultStatusCode);

        var errorResult = Result.Error();
        var errorWithTitle = Result.Error("Error message");

        Assert.Equal(StatusCodes.Status500InternalServerError, errorResult.ErrorValue.Status);
        Assert.Equal(StatusCodes.Status500InternalServerError, errorWithTitle.ErrorValue.Status);
    }

    [Fact]
    public void DefaultErrorStatusCodeCanBeOverridden()
    {
        CleanResultConfiguration.Options.Errors.DefaultStatusCode = StatusCodes.Status418ImATeapot;

        var errorResult = Result.Error();
        var errorWithTitle = Result.Error("Error message");

        Assert.Equal(StatusCodes.Status418ImATeapot, errorResult.ErrorValue.Status);
        Assert.Equal(StatusCodes.Status418ImATeapot, errorWithTitle.ErrorValue.Status);
    }

    [Fact]
    public void DefaultUnknownTitleHasDefaultValue()
    {
        Assert.Equal("Unknown error", CleanResultConfiguration.Options.Errors.DefaultUnknownTitle);

        var errorResult = Result.Error();

        Assert.Equal("Unknown error", errorResult.ErrorValue.Title);
    }

    [Fact]
    public void DefaultUnknownTitleCanBeOverridden()
    {
        CleanResultConfiguration.Options.Errors.DefaultUnknownTitle = "Configured unknown error";

        var errorResult = Result.Error();

        Assert.Equal("Configured unknown error", errorResult.ErrorValue.Title);
    }

    [Fact]
    public async Task SuccessSerializerHasDefaultValue()
    {
        Assert.Equal(
            """{"firstName":"Ada"}""",
            CleanResultConfiguration.Options.AspNetCore.SuccessSerializer(new { FirstName = "Ada" })
        );

        var httpContext = HttpContextUtils.GetHttpContext();
        await Result.Ok(new { FirstName = "Ada" }).ExecuteAsync(httpContext);

        Assert.Equal("""{"firstName":"Ada"}""", HttpContextUtils.ReadContextBody(httpContext));
    }

    [Fact]
    public async Task SuccessSerializerCanBeOverridden()
    {
        CleanResultConfiguration.Options.AspNetCore.SuccessSerializer = _ => "custom-success";

        var httpContext = HttpContextUtils.GetHttpContext();
        await Result.Ok(new { FirstName = "Ada" }).ExecuteAsync(httpContext);

        Assert.Equal("custom-success", HttpContextUtils.ReadContextBody(httpContext));
    }

    [Fact]
    public async Task ErrorSerializerHasDefaultValue()
    {
        var error = Result.Error("Error message", StatusCodes.Status400BadRequest).ErrorValue;
        var expectedJson =
            """{"type":"https://tools.ietf.org/html/rfc7231#section-6.5.1","title":"Error message","status":400}""";

        Assert.Equal(expectedJson, CleanResultConfiguration.Options.AspNetCore.ErrorSerializer(error));

        var httpContext = HttpContextUtils.GetHttpContext();
        await Result.Error("Error message", StatusCodes.Status400BadRequest).ExecuteAsync(httpContext);

        Assert.Equal(expectedJson, HttpContextUtils.ReadContextBody(httpContext));
    }

    [Fact]
    public async Task ErrorSerializerCanBeOverridden()
    {
        CleanResultConfiguration.Options.AspNetCore.ErrorSerializer = value =>
        {
            var error = (Error)value;
            return $"custom-error:{error.Status}:{error.Title}";
        };

        var httpContext = HttpContextUtils.GetHttpContext();
        await Result.Error("Error message", StatusCodes.Status400BadRequest).ExecuteAsync(httpContext);

        Assert.Equal("custom-error:400:Error message", HttpContextUtils.ReadContextBody(httpContext));
    }

    [Fact]
    public async Task AddCleanResultConfiguresGlobalOptions()
    {
        var services = new ServiceCollection();

        services.AddCleanResult(options =>
        {
            options.AspNetCore.DefaultValueSuccessStatusCode = StatusCodes.Status201Created;
            options.Errors.DefaultUnknownTitle = "Unexpected error";
        });

        Assert.Equal(StatusCodes.Status201Created, CleanResultConfiguration.Options.AspNetCore.DefaultValueSuccessStatusCode);
        Assert.Equal("Unexpected error", CleanResultConfiguration.Options.Errors.DefaultUnknownTitle);

        var successContext = HttpContextUtils.GetHttpContext();
        await Result.Ok(new { Message = "Created" }).ExecuteAsync(successContext);

        Assert.Equal(StatusCodes.Status201Created, successContext.Response.StatusCode);
        Assert.Equal("Unexpected error", Result.Error().ErrorValue.Title);
    }

    [Fact]
    public void AddCleanResultReturnsSameServiceCollection()
    {
        var services = new ServiceCollection();

        var returnedServices = services.AddCleanResult(_ => { });

        Assert.Same(services, returnedServices);
    }

    [Fact]
    public void AddCleanResultThrowsWhenServicesIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            CleanResultServiceCollectionExtensions.AddCleanResult(null!, _ => { })
        );
    }

    [Fact]
    public void AddCleanResultThrowsWhenConfigureIsNull()
    {
        var services = new ServiceCollection();

        Assert.Throws<ArgumentNullException>(() => services.AddCleanResult(null!));
    }

    private static void ResetConfiguration()
    {
        CleanResultConfiguration.Options.AspNetCore.DefaultSimpleSuccessStatusCode = StatusCodes.Status204NoContent;
        CleanResultConfiguration.Options.AspNetCore.DefaultValueSuccessStatusCode = StatusCodes.Status200OK;
        CleanResultConfiguration.Options.AspNetCore.JsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        CleanResultConfiguration.Options.AspNetCore.SuccessSerializer = value =>
            JsonSerializer.Serialize(value, CleanResultConfiguration.Options.AspNetCore.JsonSerializerOptions);
        CleanResultConfiguration.Options.AspNetCore.ErrorSerializer = value =>
            JsonSerializer.Serialize(value, CleanResultConfiguration.Options.AspNetCore.JsonSerializerOptions);

        CleanResultConfiguration.Options.Errors.DefaultStatusCode = StatusCodes.Status500InternalServerError;
        CleanResultConfiguration.Options.Errors.DefaultUnknownTitle = "Unknown error";
    }
}
