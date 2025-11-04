using FluentValidation.Results;

namespace CleanResult.FluentValidation;

/// <summary>
/// Extension methods for converting FluentValidation ValidationResult to CleanResult Result types.
/// </summary>
public static class ValidationResultExtensions
{
    /// <summary>
    /// Converts a FluentValidation ValidationResult to a CleanResult Result.
    /// </summary>
    /// <param name="validationResult">The FluentValidation ValidationResult to convert.</param>
    /// <param name="title">The title for the error. Defaults to "Validation Failed".</param>
    /// <param name="detail">Optional detail message for the error.</param>
    /// <param name="instance">Optional instance URI for the error.</param>
    /// <returns>Result.Ok() if validation succeeded, otherwise Result.Error() with validation errors.</returns>
    public static Result ToResult(
        this ValidationResult validationResult,
        string title = "Validation Failed",
        string? detail = null,
        string? instance = null)
    {
        if (validationResult.IsValid)
            return Result.Ok();

        var error = CreateError(validationResult, title, detail, instance);
        return Result.Error(error);
    }

    /// <summary>
    /// Converts a FluentValidation ValidationResult to a CleanResult Result&lt;T&gt;.
    /// </summary>
    /// <typeparam name="T">The type of the value for successful results.</typeparam>
    /// <param name="validationResult">The FluentValidation ValidationResult to convert.</param>
    /// <param name="value">The value to return if validation succeeded.</param>
    /// <param name="title">The title for the error. Defaults to "Validation Failed".</param>
    /// <param name="detail">Optional detail message for the error.</param>
    /// <param name="instance">Optional instance URI for the error.</param>
    /// <returns>Result.Ok(value) if validation succeeded, otherwise Result.Error() with validation errors.</returns>
    public static Result<T> ToResult<T>(
        this ValidationResult validationResult,
        T value,
        string title = "Validation Failed",
        string? detail = null,
        string? instance = null)
    {
        if (validationResult.IsValid)
            return Result.Ok(value);

        var error = CreateError(validationResult, title, detail, instance);
        return Result<T>.Error(error);
    }


    /// <summary>
    /// Creates an Error from a ValidationResult with all validation failures.
    /// </summary>
    /// <param name="validationResult">The validation result containing failures.</param>
    /// <param name="title">The title for the error.</param>
    /// <param name="detail">Optional detail message for the error.</param>
    /// <param name="instance">Optional instance URI for the error.</param>
    /// <returns>An Error object with status 400 (Bad Request) and all validation failures.</returns>
    private static Error CreateError(
        ValidationResult validationResult,
        string title,
        string? detail,
        string? instance)
    {
        var errors = validationResult.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray()
            );

        return new Error
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = title,
            Status = 400,
            Detail = detail,
            Instance = instance,
            Errors = errors
        };
    }
}