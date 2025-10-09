using System.Net;

namespace CleanResult.WolverineFx;

public static class ErrorMapper
{
    /// <summary>
    /// Returns true if the result is an error or if the value is null.
    /// </summary>
    /// <param name="result">The Result to check.</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool IsQueryError<T>(this Result<T> result)
    {
        return result.IsError() || result.Value is null;
    }

    /// <summary>
    /// Maps Result, returning a NotFound error if the value is null, or the original error if it is an error.
    /// Throws InvalidOperationException if the result is Ok with a non-null value.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="notFoundErrorMessage"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static Result ToQueryError<T>(this Result<T> result, string notFoundErrorMessage)
    {
        if (result.IsError())
            return Result.From(result);

        if (result.Value is null)
            return Result.Error(notFoundErrorMessage, HttpStatusCode.NotFound);

        throw new InvalidOperationException("Result is not an error");
    }
}