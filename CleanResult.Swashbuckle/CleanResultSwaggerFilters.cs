using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CleanResult.Swashbuckle;

/// <summary>
/// This filter modifies the OpenAPI operation responses to use CleanResult types.
/// </summary>
internal class CleanResultReturnTypeFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var returnType = context.MethodInfo.ReturnType;
        if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
            returnType = returnType.GetGenericArguments()[0];

        if (returnType == typeof(Result))
        {
            operation.Responses.Remove("200");
            operation.Responses.Add("204", new OpenApiResponse
            {
                Description = "Success",
                Content = null
            });
        }

        if (returnType.IsGenericType &&
            returnType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            operation.Responses.Remove("200");
            operation.Responses.Add("200", new OpenApiResponse
            {
                Description = "Success",
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = new()
                    {
                        Schema = context.SchemaGenerator.GenerateSchema(
                            returnType.GetGenericArguments()[0], context.SchemaRepository)
                    }
                }
            });
        }
    }
}

/// <summary>
/// This filter marks the schemas that are generated CleanResult with "SchemaToDelete" title.
/// </summary>
internal class CleanResultSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(Result) ||
            (context.Type.IsGenericType && context.Type.GetGenericTypeDefinition() == typeof(Result<>)))
            schema.Title = "SchemaToDelete";
    }
}

/// <summary>
/// This filter removes the schemas that are marked with "SchemaToDelete" title form the OpenAPI document.
/// </summary>
internal class CleanResultReturnDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var keysToRemove = swaggerDoc.Components.Schemas
            .Where(kvp => kvp.Value.Title == "SchemaToDelete")
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in keysToRemove)
            swaggerDoc.Components.Schemas.Remove(key);
    }
}

/// <summary>
/// Extension methods for SwaggerGenOptions to add CleanResult filters.
/// </summary>
public static class CleanResultSwaggerExtension
{
    /// <summary>
    /// Registers the CleanResult filters for Swagger generation.
    /// </summary>
    /// <param name="options"> The SwaggerGenOptions to which the filters will be added. </param>
    /// <returns>updated SwaggerGenOptions</returns>
    public static SwaggerGenOptions AddCleanResultFilters(this SwaggerGenOptions options)
    {
        options.OperationFilter<CleanResultReturnTypeFilter>();
        options.SchemaFilter<CleanResultSchemaFilter>();
        options.DocumentFilter<CleanResultReturnDocumentFilter>();
        return options;
    }
}