using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CleanResult.Swashbuckle.IntegrationTests;

public class SwaggerDocumentTests : IClassFixture<SwaggerAppFactory>
{
    private readonly HttpClient _client;

    public SwaggerDocumentTests(SwaggerAppFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });
    }

    [Fact]
    public async Task SwaggerDocumentOmitsCleanResultWrapperSchemas()
    {
        var response = await _client.GetAsync("/swagger/v1/swagger.json");
        response.EnsureSuccessStatusCode();

        var document = JsonNode.Parse(await response.Content.ReadAsStringAsync())!.AsObject();
        var schemas = document["components"]!["schemas"]!.AsObject();

        Assert.DoesNotContain(schemas, schema => schema.Key == "Result" || schema.Key.StartsWith("ResultOf", StringComparison.Ordinal));
    }

    [Fact]
    public async Task SwaggerDocumentUnwrapsGenericResultResponses()
    {
        var response = await _client.GetAsync("/swagger/v1/swagger.json");
        response.EnsureSuccessStatusCode();

        var document = JsonNode.Parse(await response.Content.ReadAsStringAsync())!.AsObject();
        var paths = document["paths"]!.AsObject();
        var intSchema = GetResponseSchema(paths, "/", "get", "200");

        Assert.Equal("integer", intSchema!["type"]?.GetValue<string>());
        Assert.Equal("#/components/schemas/CustomResponse", GetResponseSchemaRef(paths, "/custom-response", "get", "200"));
        Assert.Equal("#/components/schemas/NullableResponse", GetResponseSchemaRef(paths, "/nullable-test2", "get", "200"));
    }

    [Fact]
    public async Task SwaggerDocumentMapsResultWithoutValueToNoContent()
    {
        var response = await _client.GetAsync("/swagger/v1/swagger.json");
        response.EnsureSuccessStatusCode();

        var document = JsonNode.Parse(await response.Content.ReadAsStringAsync())!.AsObject();
        var responses = document["paths"]!["/no-content"]!["get"]!["responses"]!.AsObject();

        Assert.True(responses.ContainsKey("204"));
        Assert.False(responses.ContainsKey("200"));
        Assert.Null(responses["204"]!["content"]);
    }

    private static string? GetResponseSchemaRef(JsonObject paths, string path, string method, string statusCode)
    {
        return paths[path]![method]!["responses"]![statusCode]!["content"]!["application/json"]!["schema"]!["$ref"]?.GetValue<string>();
    }

    private static JsonNode? GetResponseSchema(JsonObject paths, string path, string method, string statusCode)
    {
        return paths[path]![method]!["responses"]![statusCode]!["content"]!["application/json"]!["schema"];
    }
}

public class SwaggerAppFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
    }
}
