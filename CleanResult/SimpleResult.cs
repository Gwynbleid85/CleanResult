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
            httpContext.Response.StatusCode = StatusCodes.Status204NoContent;
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
    /// <returns>Result object representing an error</returns>
    public static Result Error(string title, int status, string? type = null,
        string? detail = null, string? instance = null)
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
                Instance = instance
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
    /// <returns>Result object representing an error</returns>
    public static Result Error(string title, HttpStatusCode status,
        string? type = null,
        string? detail = null, string? instance = null)
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
                Instance = instance
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