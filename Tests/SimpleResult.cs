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
        Assert.Equal(0, result.ErrorValue.Code);
        Assert.Equal(String.Empty, result.ErrorValue.Message);
    }

    [Fact]
    public void ErrorResultWithErrorMessage()
    {
        var result = Result.Error("Error message");

        Assert.True(result.IsError());
        Assert.False(result.IsOk());
        Assert.Equal(0, result.ErrorValue.Code);
        Assert.Equal("Error message", result.ErrorValue.Message);
    }

    [Fact]
    public void ErrorResultWithErrorMessageAndCode()
    {
        var result = Result.Error("Error message", 404);

        Assert.True(result.IsError());
        Assert.False(result.IsOk());
        Assert.Equal(404, result.ErrorValue.Code);
        Assert.Equal("Error message", result.ErrorValue.Message);
    }

    [Fact]
    public void ErrorResultWithErrorMessageAndHttpStatusCode()
    {
        var result = Result.Error("Error message", System.Net.HttpStatusCode.NotFound);

        Assert.True(result.IsError());
        Assert.False(result.IsOk());
        Assert.Equal(404, result.ErrorValue.Code);
        Assert.Equal("Error message", result.ErrorValue.Message);
    }

    [Fact]
    public void ErrorResultWithErrorObject()
    {
        var error = new Error { Message = "Error message", Code = 500 };
        var result = Result.Error(error);

        Assert.True(result.IsError());
        Assert.False(result.IsOk());
        Assert.Equal(500, result.ErrorValue.Code);
        Assert.Equal("Error message", result.ErrorValue.Message);
    }
}