using System.Text.Json;
using CleanResult;
using Xunit.Abstractions;

namespace Tests;

public class Serialization(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void SerializeOkSimpleResult()
    {
        var result = Result.Ok();
        var serialized = JsonSerializer.Serialize(result);

        testOutputHelper.WriteLine(serialized);
        Assert.Equal("""{"Success":true}""", serialized);
    }

    [Fact]
    public void SerializeErrorSimpleResult()
    {
        var result = Result.Error("An error occurred", 123);
        var serialized = JsonSerializer.Serialize(result);

        testOutputHelper.WriteLine(serialized);
        Assert.Equal(
            """{"Success":false,"ErrorValue":{"type":"https://tools.ietf.org/html/rfc7231","title":"An error occurred","status":123}}""",
            serialized);
    }

    [Fact]
    public void SerializeOkStringValueResult()
    {
        var result = Result.Ok("Test Value");
        var serialized = JsonSerializer.Serialize(result);

        testOutputHelper.WriteLine(serialized);
        Assert.Equal("""{"Success":true,"SuccessValue":"Test Value"}""", serialized);
    }

    [Fact]
    public void SerializeOkTestStructValueResult()
    {
        var testStruct = new TestStruct { Id = 1, Name = "Test" };
        var result = Result.Ok(testStruct);
        var serialized = JsonSerializer.Serialize(result);

        testOutputHelper.WriteLine(serialized);
        Assert.Equal("""{"Success":true,"SuccessValue":{"Id":1,"Name":"Test"}}""", serialized);
    }

    [Fact]
    public void SerializeErrorStringValueResult()
    {
        var result = Result<string>.Error("Error message", 404);
        var serialized = JsonSerializer.Serialize(result);

        testOutputHelper.WriteLine(serialized);
        Assert.Equal(
            """{"Success":false,"ErrorValue":{"type":"https://tools.ietf.org/html/rfc7231#section-6.5.4","title":"Error message","status":404}}""",
            serialized);
    }

    [Fact]
    public void SerializeErrorIntValueResult()
    {
        var result = Result<int>.Error("Error message", 123);
        var serialized = JsonSerializer.Serialize(result);

        testOutputHelper.WriteLine(serialized);
        Assert.Equal(
            """{"Success":false,"ErrorValue":{"type":"https://tools.ietf.org/html/rfc7231","title":"Error message","status":123}}""",
            serialized);
    }

    [Fact]
    public void SerializeErrorTestStructValueResult()
    {
        var error = new Error { Type = "https://asdf.com", Title = "Error message", Status = 123 };
        var result = Result<TestStruct>.Error(error);
        var serialized = JsonSerializer.Serialize(result);

        testOutputHelper.WriteLine(serialized);
        Assert.Equal(
            """{"Success":false,"ErrorValue":{"type":"https://asdf.com","title":"Error message","status":123}}""",
            serialized);
    }

    [Fact]
    public void DeserializeOkSimpleResult()
    {
        var json = """{"Success":true}""";
        var result = JsonSerializer.Deserialize<Result>(json);

        Assert.NotNull(result);
        Assert.True(result.IsOk());
        Assert.False(result.IsError());
    }

    [Fact]
    public void DeserializeErrorSimpleResult()
    {
        var json = """{"Success":false,"ErrorValue":{"type":"","title":"An error occurred","status":123}}""";
        var result = JsonSerializer.Deserialize<Result>(json);

        Assert.NotNull(result);
        Assert.True(result.IsError());
        Assert.False(result.IsOk());
        Assert.Equal("An error occurred", result.ErrorValue.Title);
        Assert.Equal(123, result.ErrorValue.Status);
    }

    [Fact]
    public void DeserializeOkStringValueResult()
    {
        var json = """{"Success":true,"SuccessValue":"Test Value"}""";
        var result = JsonSerializer.Deserialize<Result<string>>(json);

        Assert.NotNull(result);
        Assert.True(result.IsOk());
        Assert.False(result.IsError());
        Assert.Equal("Test Value", result.Value);
    }

    [Fact]
    public void DeserializeOkTestStructValueResult()
    {
        var json = """{"Success":true,"SuccessValue":{"Id":1,"Name":"Test"}}""";
        var result = JsonSerializer.Deserialize<Result<TestStruct>>(json);

        Assert.NotNull(result);
        Assert.True(result.IsOk());
        Assert.False(result.IsError());
        Assert.Equal(1, result.Value.Id);
        Assert.Equal("Test", result.Value.Name);
    }

    [Fact]
    public void DeserializeErrorStringValueResult()
    {
        var json = """{"Success":false,"ErrorValue":{"type":"","title":"Error message","status":123}}""";
        var result = JsonSerializer.Deserialize<Result<string>>(json);

        Assert.NotNull(result);
        Assert.True(result.IsError());
        Assert.False(result.IsOk());
        Assert.Equal("Error message", result.ErrorValue.Title);
        Assert.Equal(123, result.ErrorValue.Status);
    }

    [Fact]
    public void DeserializeErrorIntValueResult()
    {
        var json = """{"Success":false,"ErrorValue":{"type":"","title":"Error message","status":123}}""";
        var result = JsonSerializer.Deserialize<Result<int>>(json);

        Assert.NotNull(result);
        Assert.True(result.IsError());
        Assert.False(result.IsOk());
        Assert.Equal("Error message", result.ErrorValue.Title);
        Assert.Equal(123, result.ErrorValue.Status);
    }

    [Fact]
    public void DeserializeErrorTestStructValueResult()
    {
        var json = """{"Success":false,"ErrorValue":{"type":"","title":"Error message","status":123}}""";
        var result = JsonSerializer.Deserialize<Result<TestStruct>>(json);

        Assert.NotNull(result);
        Assert.True(result.IsError());
        Assert.False(result.IsOk());
        Assert.Equal("Error message", result.ErrorValue.Title);
        Assert.Equal(123, result.ErrorValue.Status);
    }
}