using System.Net;
using CleanResult;

namespace Tests;

public class SimpleError
{
    [Fact]
    public void OkResultWithoutValue()
    {
        var result = Result.Ok();

        Assert.True(result.IsOk());
        Assert.False(result.IsError());
    }

    [Fact]
    public void ErrorResultValue()
    {
        var result = Result.Error();

        Assert.True(result.IsError());
        Assert.False(result.IsOk());
        Assert.Equal(500, result.ErrorValue.Status);
        Assert.Equal("Unknown error", result.ErrorValue.Title);
    }

    [Fact]
    public void ErrorResultWithErrorMessage()
    {
        var result = Result.Error("Error message");

        Assert.True(result.IsError());
        Assert.False(result.IsOk());
        Assert.Equal(500, result.ErrorValue.Status);
        Assert.Equal("Error message", result.ErrorValue.Title);
    }

    [Fact]
    public void ErrorResultWithErrorMessageAndCode()
    {
        var result = Result.Error("Error message", 404);

        Assert.True(result.IsError());
        Assert.False(result.IsOk());
        Assert.Equal(404, result.ErrorValue.Status);
        Assert.Equal("Error message", result.ErrorValue.Title);
    }

    [Fact]
    public void ErrorResultWithErrorMessageAndHttpStatusCode()
    {
        var result = Result.Error("Error message", HttpStatusCode.NotFound);

        Assert.True(result.IsError());
        Assert.False(result.IsOk());
        Assert.Equal(404, result.ErrorValue.Status);
        Assert.Equal("Error message", result.ErrorValue.Title);
    }

    [Fact]
    public void ErrorResultWithErrorObject()
    {
        var error = new Error { Title = "Error message", Status = 500 };
        var result = Result.Error(error);

        Assert.True(result.IsError());
        Assert.False(result.IsOk());
        Assert.Equal(500, result.ErrorValue.Status);
        Assert.Equal("Error message", result.ErrorValue.Title);
    }
}