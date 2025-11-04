using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CleanResult.Swashbuckle.Tests;

/// <summary>
/// Schema filter to make all properties required in swagger
/// </summary>
public class MakeAllPropertiesRequiredFilter : ISchemaFilter
{
    /// <summary>
    /// Applies the filter to make all properties required in the OpenAPI schema.
    /// </summary>
    /// <param name="schema">The OpenAPI schema to modify.</param>
    /// <param name="context">The schema filter context.</param>
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type != typeof(Error))
            schema.Required = schema.Properties.Keys.ToHashSet();
    }
}