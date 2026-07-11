using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace CleanResult;

/// <summary>
/// Provides global configuration for CleanResult behavior.
/// </summary>
public static class CleanResultConfiguration
{
    /// <summary>
    /// Configuration for ASP.NET Core response handling.
    /// </summary>
    public static class AspNetCore
    {
        /// <summary>
        /// Default HTTP status code for successful <see cref="CleanResult.Result" /> responses.
        /// </summary>
        public static int DefaultSimpleSuccessStatusCode { get; set; } = StatusCodes.Status204NoContent;

        /// <summary>
        /// Default HTTP status code for successful <see cref="CleanResult.Result{T}" /> responses.
        /// </summary>
        public static int DefaultValueSuccessStatusCode { get; set; } = StatusCodes.Status200OK;

        /// <summary>
        /// Serializes the success value when running <see cref="CleanResult.Result.ExecuteAsync" />.
        /// </summary>
        public static Func<object?, string> SuccessSerializationFunction { get; set; } =
            value =>
                JsonSerializer.Serialize(
                    value,
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
                );

        /// <summary>
        /// Serializes the error value when running <see cref="CleanResult.Result.ExecuteAsync" />.
        /// </summary>
        public static Func<object, string> ErrorSerializationFunction { get; set; } =
            value => JsonSerializer.Serialize(value);
    }

    /// <summary>
    /// Configuration for error defaults.
    /// </summary>
    public static class Errors
    {
        /// <summary>
        /// Default HTTP status code used when an error result does not specify one.
        /// </summary>
        public static int DefaultStatusCode { get; set; } = StatusCodes.Status500InternalServerError;

        /// <summary>
        /// Default title used when an error result does not specify one.
        /// </summary>
        public static string DefaultUnknownTitle { get; set; } = "Unknown error";
    }
}
