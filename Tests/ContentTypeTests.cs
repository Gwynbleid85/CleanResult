using System.Xml;
using System.Xml.Linq;
using CleanResult;
using Microsoft.AspNetCore.Http;
using Tests.Utils;

namespace Tests;

public class ContentTypeTests
{
    [Fact]
    public async Task StringResult_ReturnsTextPlainContentType()
    {
        var result = Result.Ok("Hello World");
        var httpContext = HttpContextUtils.GetHttpContext();

        await result.ExecuteAsync(httpContext);

        Assert.Equal(StatusCodes.Status200OK, httpContext.Response.StatusCode);
        Assert.Equal("text/plain; charset=utf-8", httpContext.Response.ContentType);
        var bodyText = HttpContextUtils.ReadContextBody(httpContext);
        Assert.Equal("Hello World", bodyText);
    }

    [Fact]
    public async Task ObjectResult_ReturnsJsonContentType()
    {
        var result = Result.Ok(new { Name = "Test", Value = 42 });
        var httpContext = HttpContextUtils.GetHttpContext();

        await result.ExecuteAsync(httpContext);

        Assert.Equal(StatusCodes.Status200OK, httpContext.Response.StatusCode);
        Assert.Equal("application/json; charset=utf-8", httpContext.Response.ContentType);
        var bodyText = HttpContextUtils.ReadContextBody(httpContext);
        Assert.Equal("""{"name":"Test","value":42}""", bodyText);
    }

    [Fact]
    public async Task IntResult_ReturnsJsonContentType()
    {
        var result = Result.Ok(42);
        var httpContext = HttpContextUtils.GetHttpContext();

        await result.ExecuteAsync(httpContext);

        Assert.Equal(StatusCodes.Status200OK, httpContext.Response.StatusCode);
        Assert.Equal("application/json; charset=utf-8", httpContext.Response.ContentType);
        var bodyText = HttpContextUtils.ReadContextBody(httpContext);
        Assert.Equal("42", bodyText);
    }

    [Fact]
    public async Task BoolResult_ReturnsJsonContentType()
    {
        var result = Result.Ok(true);
        var httpContext = HttpContextUtils.GetHttpContext();

        await result.ExecuteAsync(httpContext);

        Assert.Equal(StatusCodes.Status200OK, httpContext.Response.StatusCode);
        Assert.Equal("application/json; charset=utf-8", httpContext.Response.ContentType);
        var bodyText = HttpContextUtils.ReadContextBody(httpContext);
        Assert.Equal("true", bodyText);
    }

    [Fact]
    public async Task ByteArrayResult_ReturnsOctetStreamContentType()
    {
        var bytes = new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F }; // "Hello" in ASCII
        var result = Result.Ok(bytes);
        var httpContext = HttpContextUtils.GetHttpContext();

        await result.ExecuteAsync(httpContext);

        Assert.Equal(StatusCodes.Status200OK, httpContext.Response.StatusCode);
        Assert.Equal("application/octet-stream", httpContext.Response.ContentType);

        var body = HttpContextUtils.ReadContextBodyAsBytes(httpContext);
        Assert.Equal(bytes, body);
    }

    [Fact]
    public async Task StreamResult_ReturnsOctetStreamContentType()
    {
        var data = "Stream content"u8.ToArray();
        var stream = new MemoryStream(data);
        var result = Result.Ok<Stream>(stream);
        var httpContext = HttpContextUtils.GetHttpContext();

        await result.ExecuteAsync(httpContext);

        Assert.Equal(StatusCodes.Status200OK, httpContext.Response.StatusCode);
        Assert.Equal("application/octet-stream", httpContext.Response.ContentType);

        var body = HttpContextUtils.ReadContextBodyAsBytes(httpContext);
        Assert.Equal(data, body);
    }

    [Fact]
    public async Task ListResult_ReturnsJsonContentType()
    {
        var list = new List<string> { "item1", "item2", "item3" };
        var result = Result.Ok(list);
        var httpContext = HttpContextUtils.GetHttpContext();

        await result.ExecuteAsync(httpContext);

        Assert.Equal(StatusCodes.Status200OK, httpContext.Response.StatusCode);
        Assert.Equal("application/json; charset=utf-8", httpContext.Response.ContentType);
        var bodyText = HttpContextUtils.ReadContextBody(httpContext);
        Assert.Equal("""["item1","item2","item3"]""", bodyText);
    }

    [Fact]
    public async Task DictionaryResult_ReturnsJsonContentType()
    {
        var dict = new Dictionary<string, int> { ["one"] = 1, ["two"] = 2 };
        var result = Result.Ok(dict);
        var httpContext = HttpContextUtils.GetHttpContext();

        await result.ExecuteAsync(httpContext);

        Assert.Equal(StatusCodes.Status200OK, httpContext.Response.StatusCode);
        Assert.Equal("application/json; charset=utf-8", httpContext.Response.ContentType);
        var bodyText = HttpContextUtils.ReadContextBody(httpContext);
        Assert.Contains("\"one\":1", bodyText);
        Assert.Contains("\"two\":2", bodyText);
    }

    [Fact]
    public async Task ErrorResult_AlwaysReturnsJsonContentType()
    {
        var result = Result<string>.Error("Test error", 400);
        var httpContext = HttpContextUtils.GetHttpContext();

        await result.ExecuteAsync(httpContext);

        Assert.Equal(StatusCodes.Status400BadRequest, httpContext.Response.StatusCode);
        Assert.Equal("application/json", httpContext.Response.ContentType);
        var bodyText = HttpContextUtils.ReadContextBody(httpContext);
        Assert.Contains("\"title\":\"Test error\"", bodyText);
    }

    [Fact]
    public async Task XDocumentResult_ReturnsXmlContentType()
    {
        var xdoc = new XDocument(
            new XElement("root",
                new XElement("item", "value1"),
                new XElement("item", "value2")
            )
        );
        var result = Result.Ok(xdoc);
        var httpContext = HttpContextUtils.GetHttpContext();

        await result.ExecuteAsync(httpContext);

        Assert.Equal(StatusCodes.Status200OK, httpContext.Response.StatusCode);
        Assert.Equal("application/xml; charset=utf-8", httpContext.Response.ContentType);
    }

    [Fact]
    public async Task XElementResult_ReturnsXmlContentType()
    {
        var xelement = new XElement("root",
            new XElement("child", "value")
        );
        var result = Result.Ok(xelement);
        var httpContext = HttpContextUtils.GetHttpContext();

        await result.ExecuteAsync(httpContext);

        Assert.Equal(StatusCodes.Status200OK, httpContext.Response.StatusCode);
        Assert.Equal("application/xml; charset=utf-8", httpContext.Response.ContentType);
    }

    [Fact]
    public async Task XmlDocumentResult_ReturnsXmlContentType()
    {
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml("<root><item>value</item></root>");
        var result = Result.Ok(xmlDoc);
        var httpContext = HttpContextUtils.GetHttpContext();

        await result.ExecuteAsync(httpContext);

        Assert.Equal(StatusCodes.Status200OK, httpContext.Response.StatusCode);
        Assert.Equal("application/xml; charset=utf-8", httpContext.Response.ContentType);
    }

    [Fact]
    public async Task DecimalResult_ReturnsJsonContentType()
    {
        var result = Result.Ok(123.45m);
        var httpContext = HttpContextUtils.GetHttpContext();

        await result.ExecuteAsync(httpContext);

        Assert.Equal(StatusCodes.Status200OK, httpContext.Response.StatusCode);
        Assert.Equal("application/json; charset=utf-8", httpContext.Response.ContentType);
        var bodyText = HttpContextUtils.ReadContextBody(httpContext);
        Assert.Equal("123.45", bodyText);
    }

    [Fact]
    public async Task DateTimeResult_ReturnsJsonContentType()
    {
        var date = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);
        var result = Result.Ok(date);
        var httpContext = HttpContextUtils.GetHttpContext();

        await result.ExecuteAsync(httpContext);

        Assert.Equal(StatusCodes.Status200OK, httpContext.Response.StatusCode);
        Assert.Equal("application/json; charset=utf-8", httpContext.Response.ContentType);
        var bodyText = HttpContextUtils.ReadContextBody(httpContext);
        Assert.Contains("2024-01-15", bodyText);
    }

    [Fact]
    public async Task GuidResult_ReturnsJsonContentType()
    {
        var guid = Guid.Parse("12345678-1234-1234-1234-123456789012");
        var result = Result.Ok(guid);
        var httpContext = HttpContextUtils.GetHttpContext();

        await result.ExecuteAsync(httpContext);

        Assert.Equal(StatusCodes.Status200OK, httpContext.Response.StatusCode);
        Assert.Equal("application/json; charset=utf-8", httpContext.Response.ContentType);
        var bodyText = HttpContextUtils.ReadContextBody(httpContext);
        Assert.Contains("12345678-1234-1234-1234-123456789012", bodyText);
    }

    [Fact]
    public async Task DoubleResult_ReturnsJsonContentType()
    {
        var result = Result.Ok(3.14159);
        var httpContext = HttpContextUtils.GetHttpContext();

        await result.ExecuteAsync(httpContext);

        Assert.Equal(StatusCodes.Status200OK, httpContext.Response.StatusCode);
        Assert.Equal("application/json; charset=utf-8", httpContext.Response.ContentType);
        var bodyText = HttpContextUtils.ReadContextBody(httpContext);
        Assert.Equal("3.14159", bodyText);
    }

    [Fact]
    public async Task LongResult_ReturnsJsonContentType()
    {
        var result = Result.Ok(9876543210L);
        var httpContext = HttpContextUtils.GetHttpContext();

        await result.ExecuteAsync(httpContext);

        Assert.Equal(StatusCodes.Status200OK, httpContext.Response.StatusCode);
        Assert.Equal("application/json; charset=utf-8", httpContext.Response.ContentType);
        var bodyText = HttpContextUtils.ReadContextBody(httpContext);
        Assert.Equal("9876543210", bodyText);
    }

    [Fact]
    public async Task EmptyStringResult_ReturnsTextPlainContentType()
    {
        var result = Result.Ok(string.Empty);
        var httpContext = HttpContextUtils.GetHttpContext();

        await result.ExecuteAsync(httpContext);

        Assert.Equal(StatusCodes.Status200OK, httpContext.Response.StatusCode);
        Assert.Equal("text/plain; charset=utf-8", httpContext.Response.ContentType);
        var bodyText = HttpContextUtils.ReadContextBody(httpContext);
        Assert.Equal(string.Empty, bodyText);
    }

    [Fact]
    public async Task EmptyByteArrayResult_ReturnsOctetStreamContentType()
    {
        var bytes = Array.Empty<byte>();
        var result = Result.Ok(bytes);
        var httpContext = HttpContextUtils.GetHttpContext();

        await result.ExecuteAsync(httpContext);

        Assert.Equal(StatusCodes.Status200OK, httpContext.Response.StatusCode);
        Assert.Equal("application/octet-stream", httpContext.Response.ContentType);
        var body = HttpContextUtils.ReadContextBodyAsBytes(httpContext);
        Assert.Empty(body);
    }

    [Fact]
    public async Task EmptyListResult_ReturnsJsonContentType()
    {
        var list = new List<string>();
        var result = Result.Ok(list);
        var httpContext = HttpContextUtils.GetHttpContext();

        await result.ExecuteAsync(httpContext);

        Assert.Equal(StatusCodes.Status200OK, httpContext.Response.StatusCode);
        Assert.Equal("application/json; charset=utf-8", httpContext.Response.ContentType);
        var bodyText = HttpContextUtils.ReadContextBody(httpContext);
        Assert.Equal("[]", bodyText);
    }

    [Fact]
    public async Task NestedObjectResult_ReturnsJsonContentType()
    {
        var nested = new
        {
            User = new { Id = 1, Name = "John" },
            Settings = new { Theme = "Dark", Language = "en" },
            Tags = new[] { "admin", "user" }
        };
        var result = Result.Ok(nested);
        var httpContext = HttpContextUtils.GetHttpContext();

        await result.ExecuteAsync(httpContext);

        Assert.Equal(StatusCodes.Status200OK, httpContext.Response.StatusCode);
        Assert.Equal("application/json; charset=utf-8", httpContext.Response.ContentType);
        var bodyText = HttpContextUtils.ReadContextBody(httpContext);
        Assert.Contains("\"user\":", bodyText);
        Assert.Contains("\"settings\":", bodyText);
        Assert.Contains("\"tags\":", bodyText);
    }

    [Fact]
    public async Task ArrayOfObjectsResult_ReturnsJsonContentType()
    {
        var array = new[]
        {
            new { Id = 1, Name = "Item1" },
            new { Id = 2, Name = "Item2" }
        };
        var result = Result.Ok(array);
        var httpContext = HttpContextUtils.GetHttpContext();

        await result.ExecuteAsync(httpContext);

        Assert.Equal(StatusCodes.Status200OK, httpContext.Response.StatusCode);
        Assert.Equal("application/json; charset=utf-8", httpContext.Response.ContentType);
        var bodyText = HttpContextUtils.ReadContextBody(httpContext);
        Assert.Contains("\"id\":1", bodyText);
        Assert.Contains("\"id\":2", bodyText);
    }

    [Fact]
    public async Task MemoryStreamResult_ReturnsOctetStreamContentType()
    {
        var data = "Memory stream content"u8.ToArray();
        using var memoryStream = new MemoryStream(data);
        var result = Result.Ok<Stream>(memoryStream);
        var httpContext = HttpContextUtils.GetHttpContext();

        await result.ExecuteAsync(httpContext);

        Assert.Equal(StatusCodes.Status200OK, httpContext.Response.StatusCode);
        Assert.Equal("application/octet-stream", httpContext.Response.ContentType);
        var body = HttpContextUtils.ReadContextBodyAsBytes(httpContext);
        Assert.Equal(data, body);
    }

    [Fact]
    public async Task ValueTupleResult_ReturnsJsonContentType()
    {
        // Note: ValueTuples don't serialize well with System.Text.Json by default
        // They serialize as empty objects because they only have fields, not properties
        var tuple = (Id: 123, Name: "Test", Active: true);
        var result = Result.Ok(tuple);
        var httpContext = HttpContextUtils.GetHttpContext();

        await result.ExecuteAsync(httpContext);

        Assert.Equal(StatusCodes.Status200OK, httpContext.Response.StatusCode);
        Assert.Equal("application/json; charset=utf-8", httpContext.Response.ContentType);
        var bodyText = HttpContextUtils.ReadContextBody(httpContext);
        Assert.Equal("{}", bodyText); // ValueTuples serialize as empty objects
    }

    [Fact]
    public async Task NullableIntWithValueResult_ReturnsJsonContentType()
    {
        int? value = 42;
        var result = Result.Ok(value);
        var httpContext = HttpContextUtils.GetHttpContext();

        await result.ExecuteAsync(httpContext);

        Assert.Equal(StatusCodes.Status200OK, httpContext.Response.StatusCode);
        Assert.Equal("application/json; charset=utf-8", httpContext.Response.ContentType);
        var bodyText = HttpContextUtils.ReadContextBody(httpContext);
        Assert.Equal("42", bodyText);
    }

    [Fact]
    public async Task NullableIntWithNullResult_ReturnsJsonContentType()
    {
        int? value = null;
        var result = Result.Ok(value);
        var httpContext = HttpContextUtils.GetHttpContext();

        await result.ExecuteAsync(httpContext);

        Assert.Equal(StatusCodes.Status200OK, httpContext.Response.StatusCode);
        Assert.Equal("application/json; charset=utf-8", httpContext.Response.ContentType);
        var bodyText = HttpContextUtils.ReadContextBody(httpContext);
        Assert.Equal("null", bodyText);
    }

    [Fact]
    public async Task StringWithSpecialCharactersResult_ReturnsTextPlain()
    {
        var result = Result.Ok("Hello \"World\" & <Special> Characters!");
        var httpContext = HttpContextUtils.GetHttpContext();

        await result.ExecuteAsync(httpContext);

        Assert.Equal(StatusCodes.Status200OK, httpContext.Response.StatusCode);
        Assert.Equal("text/plain; charset=utf-8", httpContext.Response.ContentType);
        var bodyText = HttpContextUtils.ReadContextBody(httpContext);
        Assert.Equal("Hello \"World\" & <Special> Characters!", bodyText);
    }

    [Fact]
    public async Task RecordResult_ReturnsJsonContentType()
    {
        var record = new TestRecord(1, "John Doe");
        var result = Result.Ok(record);
        var httpContext = HttpContextUtils.GetHttpContext();

        await result.ExecuteAsync(httpContext);

        Assert.Equal(StatusCodes.Status200OK, httpContext.Response.StatusCode);
        Assert.Equal("application/json; charset=utf-8", httpContext.Response.ContentType);
        var bodyText = HttpContextUtils.ReadContextBody(httpContext);
        Assert.Contains("\"id\":1", bodyText);
        Assert.Contains("\"name\":\"John Doe\"", bodyText);
    }
}

public record TestRecord(int Id, string Name);