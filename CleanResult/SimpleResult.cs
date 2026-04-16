using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace CleanResult;

/// <summary>
/// This class represents the result of an operation.
/// It is inspired by Rust's Result type.
/// </summary>
public class Result : IResult
{
    [JsonInclude]
    internal bool Success { get; set; }

    [JsonInclude]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    internal int? SuccessStatus { get; init; }

    [JsonInclude]
    [JsonPropertyName("ErrorValue")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    internal Error? InternalErrorValue { get; init; }

    [JsonIgnore]
    public Error ErrorValue => !Success
        ? InternalErrorValue ?? new Error
        {
            Type = ProblemDetailsTypeMappings.GetProblemType(500),
            Title = "Unknown error",
            Status = StatusCodes.Status500InternalServerError
        }
        : throw new InvalidOperationException("Result is not an error");

    /// <summary>
    /// IResult interface implementation to allow using in the same way as IResult.
    /// </summary>
    public async Task ExecuteAsync(HttpContext httpContext)
    {
        if (IsOk())
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = SuccessStatus ?? StatusCodes.Status204NoContent;
            return;
        }

        // Error
        httpContext.Response.StatusCode = ErrorValue.Status;
        httpContext.Response.ContentType = "application/json";
        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(InternalErrorValue));
    }


    /// <summary>
    /// Creates a new Result object representing success.
    /// </summary>
    /// <returns>Result object representing success</returns>
    public static Result Ok()
    {
        return new Result { Success = true };
    }

    /// <summary>
    /// Creates a new Result object representing success with a custom HTTP status code and no body.
    /// </summary>
    /// <param name="statusCode">The HTTP status code returned by <see cref="ExecuteAsync"/></param>
    /// <returns>Result object representing success</returns>
    /// <remarks>
    /// A single-argument <c>Ok(int)</c> overload is intentionally not provided because it would
    /// collide with the generic <see cref="Ok{T}(T)"/> forwarder and silently reroute
    /// <c>Result.Ok(42)</c> to the no-body overload.
    /// </remarks>
    public static Result Ok(HttpStatusCode statusCode)
    {
        return new Result { Success = true, SuccessStatus = (int)statusCode };
    }

    /// <summary>
    /// Helper function to automatically infer generics type with explicitly specifying it
    /// </summary>
    /// <param name="value">Value of the success</param>
    /// <typeparam name="T">Type for the generics Result</typeparam>
    /// <returns></returns>
    public static Result<T> Ok<T>(T value)
    {
        return Result<T>.Ok(value);
    }

    /// <summary>
    /// Creates a new generics Result object representing success with a custom HTTP status code.
    /// </summary>
    /// <param name="value">Value of the success</param>
    /// <param name="statusCode">The HTTP status code returned by <see cref="Result{T}.ExecuteAsync"/></param>
    /// <typeparam name="T">Type for the generics Result</typeparam>
    /// <returns>Result object representing success</returns>
    public static Result<T> Ok<T>(T value, int statusCode)
    {
        return Result<T>.Ok(value, statusCode);
    }

    /// <summary>
    /// Creates a new generics Result object representing success with a custom HTTP status code.
    /// </summary>
    /// <param name="value">Value of the success</param>
    /// <param name="statusCode">The HTTP status code returned by <see cref="Result{T}.ExecuteAsync"/></param>
    /// <typeparam name="T">Type for the generics Result</typeparam>
    /// <returns>Result object representing success</returns>
    public static Result<T> Ok<T>(T value, HttpStatusCode statusCode)
    {
        return Result<T>.Ok(value, statusCode);
    }

    /// <summary>
    /// Function to convert generics Result to non-generics result
    /// </summary>
    /// <param name="result"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <remarks> When mapping success result the successValue will be lost</remarks>
    public static Result From<T>(Result<T> result)
    {
        return new Result
        {
            Success = result.Success,
            InternalErrorValue = result.InternalErrorValue
        };
    }

    /// <summary>
    /// Creates a new Result object representing an error without description.
    /// </summary>
    /// <returns>Result object representing an error</returns>
    public static Result Error()
    {
        return new Result { Success = false };
    }

    /// <summary>
    /// Creates a new Result object representing an error.
    /// </summary>
    /// <param name="title">A short, human-readable summary of the problem type</param>
    /// <returns>Result object representing an error</returns>
    public static Result Error(string title)
    {
        return new Result
        {
            Success = false,
            InternalErrorValue = new Error
            {
                Type = ProblemDetailsTypeMappings.GetProblemType(500),
                Title = title,
                Status = (int)HttpStatusCode.InternalServerError
            }
        };
    }

    /// <summary>
    /// Creates a new Result object representing an error.
    /// </summary>
    /// <param name="title">A short, human-readable summary of the problem type</param>
    /// <param name="status">The HTTP status code</param>
    /// <param name="type">A URI reference that identifies the problem type</param>
    /// <param name="detail">A human-readable explanation specific to this occurrence of the problem</param>
    /// <param name="instance">A URI reference that identifies the specific occurrence of the problem</param>
    /// <param name="errors">Dictionary od additional errors</param>
    /// <returns>Result object representing an error</returns>
    public static Result Error(string title, int status, string? type = null,
        string? detail = null, string? instance = null, IDictionary<string, string[]>? errors = null)
    {
        return new Result
        {
            Success = false,
            InternalErrorValue = new Error
            {
                Type = type ?? ProblemDetailsTypeMappings.GetProblemType(status),
                Title = title,
                Status = status,
                Detail = detail,
                Instance = instance,
                Errors = errors
            }
        };
    }


    /// <summary>
    /// Creates a new Result object representing an error.
    /// </summary>
    /// <param name="title">A short, human-readable summary of the problem type</param>
    /// <param name="status">The HTTP status code</param>
    /// <param name="type">A URI reference that identifies the problem type</param>
    /// <param name="detail">A human-readable explanation specific to this occurrence of the problem</param>
    /// <param name="instance">A URI reference that identifies the specific occurrence of the problem</param>
    /// <param name="errors">Dictionary od additional errors</param>
    /// <returns>Result object representing an error</returns>
    public static Result Error(string title, HttpStatusCode status,
        string? type = null,
        string? detail = null, string? instance = null, IDictionary<string, string[]>? errors = null)
    {
        return new Result
        {
            Success = false,
            InternalErrorValue = new Error
            {
                Type = type ?? ProblemDetailsTypeMappings.GetProblemType(500),
                Title = title,
                Status = (int)status,
                Detail = detail,
                Instance = instance,
                Errors = errors
            }
        };
    }

    /// <summary>
    /// Creates a new Result object representing an error.
    /// </summary>
    /// <param name="error">Error to build result from</param>
    /// <returns>Result object representing an error</returns>
    public static Result Error(Error error)
    {
        return new Result { Success = false, InternalErrorValue = error.Clone() };
    }

    /// <summary>
    /// Check if the result object represents success.
    /// </summary>
    /// <returns>`true` if object represents success, otherwise `false`</returns>
    public bool IsOk()
    {
        return Success;
    }

    /// <summary>
    /// Check if the result object represents error.
    /// </summary>
    /// <returns>`true` if object represents error, otherwise `false`</returns>
    public bool IsError()
    {
        return !Success;
    }
}