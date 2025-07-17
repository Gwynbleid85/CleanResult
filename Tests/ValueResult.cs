using CleanResult;

namespace Tests;

internal struct TestStruct
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class ValueResult
{
    [Fact]
    public void OkResultWithStringValue()
    {
        var result = Result.Ok("Test Value");

        Assert.True(result.IsOk());
        Assert.False(result.IsError());
        Assert.Equal(typeof(string), result.Value.GetType());
        Assert.Equal("Test Value", result.Value);
    }

    [Fact]
    public void OkResultWithIntValue()
    {
        var result = Result.Ok(42);

        Assert.True(result.IsOk());
        Assert.False(result.IsError());
        Assert.Equal(typeof(int), result.Value.GetType());
        Assert.Equal(42, result.Value);
    }

    [Fact]
    public void OkResultWithStructValue()
    {
        var testStruct = new TestStruct { Id = 1, Name = "Test" };
        var result = Result.Ok(testStruct);

        Assert.True(result.IsOk());
        Assert.False(result.IsError());
        Assert.Equal(typeof(TestStruct), result.Value.GetType());
        Assert.Equal(testStruct, result.Value);
    }

    [Fact]
    public void ErrorResultWithStringValue()
    {
        var result = Result<string>.Error("Error message");

        Assert.True(result.IsError());
        Assert.False(result.IsOk());
        Assert.Equal(500, result.ErrorValue.Status);
        Assert.Equal("Error message", result.ErrorValue.Title);
    }

    [Fact]
    public void ErrorResultWithIntValue()
    {
        var result = Result<int>.Error("Error message", 404);

        Assert.True(result.IsError());
        Assert.False(result.IsOk());
        Assert.Equal(404, result.ErrorValue.Status);
        Assert.Equal("Error message", result.ErrorValue.Title);
    }

    [Fact]
    public void ErrorResultWithStructValue()
    {
        var error = new Error { Title = "Error message", Status = 500 };
        var result = Result<TestStruct>.Error(error);

        Assert.True(result.IsError());
        Assert.False(result.IsOk());
        Assert.Equal(500, result.ErrorValue.Status);
        Assert.Equal("Error message", result.ErrorValue.Title);
    }
}