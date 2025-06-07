using System.Net;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace CleanResult;

/// <summary>
/// This class represents the result of an operation.
/// It is inspired by Rust's Result type.
/// </summary>
/// <remarks>
/// This Type also implements <see cref="IActionResult" />, so it can be user as a return type in endpoints
/// </remarks>
public partial class Result
{
    [JsonInclude]
    internal bool Success { get; set; }

    [JsonInclude]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    internal string? ErrorMessage { get; set; }

    [JsonInclude]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    internal int? ErrorCode { get; set; }

    [JsonIgnore]
    public Error ErrorValue => !Success
        ? new Error { Message = ErrorMessage ?? string.Empty, Code = ErrorCode ?? 0 }
        : throw new InvalidOperationException("Result is not an error");


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
            ErrorMessage = result.ErrorMessage,
            ErrorCode = result.ErrorCode
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
    /// <param name="errorMessage">Error message</param>
    /// <param name="errorCode">Error code</param>
    /// <returns>Result object representing an error</returns>
    public static Result Error(string errorMessage, int errorCode)
    {
        return new Result { Success = false, ErrorMessage = errorMessage, ErrorCode = errorCode };
    }

    /// <summary>
    /// Creates a new Result object representing an error.
    /// </summary>
    /// <param name="errorMessage">Error message</param>
    /// <param name="errorCode">Error code</param>
    /// <returns>Result object representing an error</returns>
    public static Result Error(string errorMessage, HttpStatusCode errorCode)
    {
        return new Result { Success = false, ErrorMessage = errorMessage, ErrorCode = (int)errorCode };
    }


    /// <summary>
    /// Creates a new Result object representing an error.
    /// </summary>
    /// <param name="errorMessage">Error message</param>
    /// <returns>Result object representing an error</returns>
    public static Result Error(string errorMessage)
    {
        return new Result { Success = false, ErrorMessage = errorMessage };
    }

    /// <summary>
    /// Creates a new Result object representing an error.
    /// </summary>
    /// <param name="errorMessage">Error to build result from</param>
    /// <returns>Result object representing an error</returns>
    public static Result Error(Error errorMessage)
    {
        return new Result { Success = false, ErrorMessage = errorMessage.Message, ErrorCode = errorMessage.Code };
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