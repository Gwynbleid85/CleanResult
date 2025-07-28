using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace CleanResult;

/// <summary>
/// This class represents the result of an operation.
/// It is inspired by Rust's Result type.
/// </summary>
public class Result<T> : IResult
{
    [JsonInclude]
    internal bool Success { get; set; }

    [JsonInclude]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    internal T? SuccessValue { get; set; }

    [JsonInclude]
    [JsonPropertyName("ErrorValue")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    internal Error? InternalErrorValue { get; init; }

    /// <summary>
    /// Value of the result if it represents success.
    /// </summary>
    /// <exception cref="InvalidOperationException">If tried to get Value and result is error </exception>
    [JsonIgnore]
    public T Value => Success ? SuccessValue! : throw new InvalidOperationException("Result is not a success");

    /// <summary>
    /// Error value of the result if it represents an error.
    /// </summary>
    /// <exception cref="InvalidOperationException">If tried to get Error value and result is ok </exception>
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
            httpContext.Response.StatusCode = StatusCodes.Status200OK;
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsync(JsonSerializer.Serialize(Value,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }));
            return;
        }

        // Error
        httpContext.Response.StatusCode = ErrorValue.Status;
        httpContext.Response.ContentType = "application/json";
        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(InternalErrorValue));
    }


    /// <summary>
    /// This method allows to implicitly convert non-generics error result to generics error result
    /// to make the code cleaner.
    /// </summary>
    /// <param name="result">Non-generics result to convert from</param>
    /// <returns>Converted generics result</returns>
    /// <remarks>
    /// This conversion should only be applied if the result represents error!
    /// If it represents success, then the generics result should be created using `Result.Ok(object)`
    /// that automatically converts to generics error if given some parameter.
    /// </remarks>
    public static implicit operator Result<T>(Result result)
    {
        return new Result<T>
        {
            Success = result.Success,
            InternalErrorValue = result.InternalErrorValue,
            SuccessValue = default
        };
    }


    public static Result<T> From<T1>(Result<T1> result)
    {
        if (result.Success)
            throw new InvalidOperationException(
                "Cannot convert one generics success result to another (only error results are possible to convert)!");

        return new Result<T>
        {
            Success = result.Success,
            InternalErrorValue = result.InternalErrorValue,
            SuccessValue = default
        };
    }


    /// <summary>
    /// Creates a new Result object representing success.
    /// </summary>
    /// <returns>Result object representing success</returns>
    public static Result<T> Ok(T value)
    {
        return new Result<T> { Success = true, SuccessValue = value };
    }

    /// <summary>
    /// Creates a new Result object representing an error.
    /// </summary>
    /// <param name="title">A short, human-readable summary of the problem type</param>
    /// <returns>Result object representing an error</returns>
    public static Result<T> Error(string title)
    {
        return new Result<T>
        {
            Success = false,
            InternalErrorValue = new Error
            {
                Type = ProblemDetailsTypeMappings.GetProblemType((int)HttpStatusCode.InternalServerError),
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
    public static Result<T> Error(string title, int status,
        string? type = null, string? detail = null, string? instance = null)
    {
        return new Result<T>
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
    public static Result<T> Error(string title, HttpStatusCode status,
        string? type = null, string? detail = null, string? instance = null)
    {
        return new Result<T>
        {
            Success = false,
            InternalErrorValue = new Error
            {
                Type = type ?? ProblemDetailsTypeMappings.GetProblemType((int)status),
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
    public static Result<T> Error(Error error)
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