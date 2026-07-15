using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace CleanResult;

/// <summary>
/// Provides global configuration for CleanResult behavior.
/// </summary>
public static class CleanResultConfiguration
{
    /// <summary>
    /// Global CleanResult options.
    /// </summary>
    public static CleanResultOptions Options { get; } = new();
}

/// <summary>
/// Global CleanResult options.
/// </summary>
public sealed class CleanResultOptions
{
    /// <summary>
    /// Configuration for ASP.NET Core response handling.
    /// </summary>
    public AspNetCoreResultOptions AspNetCore { get; } = new();

    /// <summary>
    /// Configuration for error defaults.
    /// </summary>
    public ErrorOptions Errors { get; } = new();
}

/// <summary>
/// Configuration for ASP.NET Core response handling.
/// </summary>
public sealed class AspNetCoreResultOptions
{
    /// <summary>
    /// Default HTTP status code for successful <see cref="CleanResult.Result" /> responses.
    /// </summary>
    public int DefaultSimpleSuccessStatusCode { get; set; } = StatusCodes.Status204NoContent;

    /// <summary>
    /// Default HTTP status code for successful <see cref="CleanResult.Result{T}" /> responses.
    /// </summary>
    public int DefaultValueSuccessStatusCode { get; set; } = StatusCodes.Status200OK;

    /// <summary>
    /// Serializes success values.
    /// </summary>
    public Func<object?, Type, string> SuccessSerializer { get; set; } =
        (value, type) => JsonSerializer.Serialize(value, type, new JsonSerializerOptions(JsonSerializerDefaults.Web));

    /// <summary>
    /// Serializes error values.
    /// </summary>
    public Func<object, string> ErrorSerializer { get; set; } =
        value => JsonSerializer.Serialize(value, new JsonSerializerOptions(JsonSerializerDefaults.Web));
}

/// <summary>
/// Configuration for error defaults.
/// </summary>
public sealed class ErrorOptions
{
    /// <summary>
    /// Default HTTP status code used when an error result does not specify one.
    /// </summary>
    public int DefaultStatusCode { get; set; } = StatusCodes.Status500InternalServerError;

    /// <summary>
    /// Default title used when an error result does not specify one.
    /// </summary>
    public string DefaultUnknownTitle { get; set; } = "Unknown error";
}
