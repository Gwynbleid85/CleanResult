using FluentValidation;

namespace CleanResult.FluentValidation;

/// <summary>
/// Extension methods for FluentValidation validators to return CleanResult Result types.
/// </summary>
public static class ValidatorExtensions
{
    /// <summary>
    /// Validates an instance and returns a Result.
    /// </summary>
    /// <typeparam name="T">The type being validated.</typeparam>
    /// <param name="validator">The validator to use.</param>
    /// <param name="instance">The instance to validate.</param>
    /// <param name="title">The title for validation errors. Defaults to "Validation Failed".</param>
    /// <param name="detail">Optional detail message for validation errors.</param>
    /// <param name="instanceUri">Optional instance URI for validation errors.</param>
    /// <returns>Result.Ok() if validation succeeds, otherwise Result.Error() with validation errors.</returns>
    public static Result ValidateToResult<T>(
        this IValidator<T> validator,
        T instance,
        string title = "Validation Failed",
        string? detail = null,
        string? instanceUri = null)
    {
        var validationResult = validator.Validate(instance);
        return validationResult.ToResult(title, detail, instanceUri);
    }

    /// <summary>
    /// Validates an instance asynchronously and returns a Result.
    /// </summary>
    /// <typeparam name="T">The type being validated.</typeparam>
    /// <param name="validator">The validator to use.</param>
    /// <param name="instance">The instance to validate.</param>
    /// <param name="title">The title for validation errors. Defaults to "Validation Failed".</param>
    /// <param name="detail">Optional detail message for validation errors.</param>
    /// <param name="instanceUri">Optional instance URI for validation errors.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result.Ok() if validation succeeds, otherwise Result.Error() with validation errors.</returns>
    public static async Task<Result> ValidateToResultAsync<T>(
        this IValidator<T> validator,
        T instance,
        string title = "Validation Failed",
        string? detail = null,
        string? instanceUri = null,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(instance, cancellationToken).ConfigureAwait(false);
        return validationResult.ToResult(title, detail, instanceUri);
    }

    /// <summary>
    /// Validates an instance and returns a Result&lt;T&gt; with the validated instance on success.
    /// </summary>
    /// <typeparam name="T">The type being validated.</typeparam>
    /// <param name="validator">The validator to use.</param>
    /// <param name="instance">The instance to validate.</param>
    /// <param name="title">The title for validation errors. Defaults to "Validation Failed".</param>
    /// <param name="detail">Optional detail message for validation errors.</param>
    /// <param name="instanceUri">Optional instance URI for validation errors.</param>
    /// <returns>Result.Ok(instance) if validation succeeds, otherwise Result.Error() with validation errors.</returns>
    public static Result<T> ValidateToResultWithValue<T>(
        this IValidator<T> validator,
        T instance,
        string title = "Validation Failed",
        string? detail = null,
        string? instanceUri = null)
    {
        var validationResult = validator.Validate(instance);
        return validationResult.ToResult(instance, title, detail, instanceUri);
    }

    /// <summary>
    /// Validates an instance asynchronously and returns a Result&lt;T&gt; with the validated instance on success.
    /// </summary>
    /// <typeparam name="T">The type being validated.</typeparam>
    /// <param name="validator">The validator to use.</param>
    /// <param name="instance">The instance to validate.</param>
    /// <param name="title">The title for validation errors. Defaults to "Validation Failed".</param>
    /// <param name="detail">Optional detail message for validation errors.</param>
    /// <param name="instanceUri">Optional instance URI for validation errors.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result.Ok(instance) if validation succeeds, otherwise Result.Error() with validation errors.</returns>
    public static async Task<Result<T>> ValidateToResultWithValueAsync<T>(
        this IValidator<T> validator,
        T instance,
        string title = "Validation Failed",
        string? detail = null,
        string? instanceUri = null,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(instance, cancellationToken).ConfigureAwait(false);
        return validationResult.ToResult(instance, title, detail, instanceUri);
    }
}