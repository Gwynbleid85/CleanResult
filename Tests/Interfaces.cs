using CleanResult;
using Microsoft.AspNetCore.Http;
using Tests.Utils;

namespace Tests;

public class Interfaces
{
    [Fact]
    public async Task ResultInterfaceMethodsWithNoContent()
    {
        var noContentResult = Result.Ok();

        var httpContext = HttpContextUtils.GetHttpContext();

        await noContentResult.ExecuteAsync(httpContext);
        Assert.Equal(StatusCodes.Status204NoContent, httpContext.Response.StatusCode);
        Assert.Equal("application/json", httpContext.Response.ContentType);
        Assert.Empty(HttpContextUtils.ReadContextBody(httpContext));
    }

    [Fact]
    public async Task ResultInterfaceMethodsSuccessResultWithSimpleValue()
    {
        var okResult = Result.Ok("Success");

        var httpContext = HttpContextUtils.GetHttpContext();

        await okResult.ExecuteAsync(httpContext);
        Assert.Equal(StatusCodes.Status200OK, httpContext.Response.StatusCode);
        Assert.Equal("application/json", httpContext.Response.ContentType);
        var bodyText = HttpContextUtils.ReadContextBody(httpContext);
        Assert.Equal("\"Success\"", bodyText);
    }

    [Fact]
    public async Task ResultInterfaceMethodsSuccessResultWithComplexValue()
    {
        var okResult = Result.Ok(new { Name = "Test", Value = 42, Compound = new { Nested = "Value" } });

        var httpContext = HttpContextUtils.GetHttpContext();

        await okResult.ExecuteAsync(httpContext);
        Assert.Equal(StatusCodes.Status200OK, httpContext.Response.StatusCode);
        Assert.Equal("application/json", httpContext.Response.ContentType);
        var bodyText = HttpContextUtils.ReadContextBody(httpContext);
        Assert.Equal("""{"name":"Test","value":42,"compound":{"nested":"Value"}}""", bodyText);
    }

    [Fact]
    public async Task ResultInterfaceMethodsWithError()
    {
        var errorResult = Result.Error("Error message", 404);

        var httpContext = HttpContextUtils.GetHttpContext();

        await errorResult.ExecuteAsync(httpContext);
        Assert.Equal(StatusCodes.Status404NotFound, httpContext.Response.StatusCode);
        Assert.Equal("application/json", httpContext.Response.ContentType);
        var bodyText = HttpContextUtils.ReadContextBody(httpContext);
        Assert.Contains("""{"title":"Error message","status":404}""", bodyText);
    }

    [Fact]
    public async Task ResultInterfaceMethodsWithComplexError()
    {
        var errorResult = Result.Error("Error message", 400, "http://example.com/error", "Detailed error message",
            "http://example.com/instance");
        var httpContext = HttpContextUtils.GetHttpContext();

        await errorResult.ExecuteAsync(httpContext);

        Assert.Equal(StatusCodes.Status400BadRequest, httpContext.Response.StatusCode);
        Assert.Equal("application/json", httpContext.Response.ContentType);
        var bodyText = HttpContextUtils.ReadContextBody(httpContext);
        Assert.Equal(
            """{"type":"http://example.com/error","title":"Error message","status":400,"detail":"Detailed error message","instance":"http://example.com/instance"}""",
            bodyText);
    }
}