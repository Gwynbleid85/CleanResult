using System.Text.Json;
using CleanResult;
using Microsoft.AspNetCore.Http;
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
    public async Task SuccessSerializationFunctionHasDefaultValue()
    {
        var httpContext = HttpContextUtils.GetHttpContext();

        await Result.Ok(new { FirstName = "Ada" }).ExecuteAsync(httpContext);

        Assert.Equal("""{"firstName":"Ada"}""", HttpContextUtils.ReadContextBody(httpContext));
    }

    [Fact]
    public async Task SuccessSerializationFunctionCanBeOverridden()
    {
        CleanResultConfiguration.Options.AspNetCore.SuccessSerializationFunction = _ => "custom-success";

        var httpContext = HttpContextUtils.GetHttpContext();
        await Result.Ok(new { FirstName = "Ada" }).ExecuteAsync(httpContext);

        Assert.Equal("custom-success", HttpContextUtils.ReadContextBody(httpContext));
    }

    [Fact]
    public async Task ErrorSerializationFunctionHasDefaultValue()
    {
        var httpContext = HttpContextUtils.GetHttpContext();

        await Result.Error("Error message", StatusCodes.Status400BadRequest).ExecuteAsync(httpContext);

        Assert.Equal(
            """{"type":"https://tools.ietf.org/html/rfc7231#section-6.5.1","title":"Error message","status":400}""",
            HttpContextUtils.ReadContextBody(httpContext));
    }

    [Fact]
    public async Task ErrorSerializationFunctionCanBeOverridden()
    {
        CleanResultConfiguration.Options.AspNetCore.ErrorSerializationFunction = value =>
        {
            var error = (Error)value;
            return $"custom-error:{error.Status}:{error.Title}";
        };

        var httpContext = HttpContextUtils.GetHttpContext();
        await Result.Error("Error message", StatusCodes.Status400BadRequest).ExecuteAsync(httpContext);

        Assert.Equal("custom-error:400:Error message", HttpContextUtils.ReadContextBody(httpContext));
    }

    private static void ResetConfiguration()
    {
        CleanResultConfiguration.Options.AspNetCore.DefaultSimpleSuccessStatusCode = StatusCodes.Status204NoContent;
        CleanResultConfiguration.Options.AspNetCore.DefaultValueSuccessStatusCode = StatusCodes.Status200OK;
        CleanResultConfiguration.Options.AspNetCore.SuccessSerializationFunction = value =>
            JsonSerializer.Serialize(
                value,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
            );
        CleanResultConfiguration.Options.AspNetCore.ErrorSerializationFunction = value => JsonSerializer.Serialize(value);

        CleanResultConfiguration.Options.Errors.DefaultStatusCode = StatusCodes.Status500InternalServerError;
        CleanResultConfiguration.Options.Errors.DefaultUnknownTitle = "Unknown error";
    }
}
