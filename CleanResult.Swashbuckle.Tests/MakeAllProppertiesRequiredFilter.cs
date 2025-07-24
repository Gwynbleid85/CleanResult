using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CleanResult.Swashbuckle.Tests;

/// <summary>
/// Schema filter to make all properties required in swagger
/// </summary>
public class MakeAllPropertiesRequiredFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type != typeof(Error))
            schema.Required = schema.Properties.Keys.ToHashSet();
    }
}