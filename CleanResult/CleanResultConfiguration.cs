using System.Text.Json;

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
}
