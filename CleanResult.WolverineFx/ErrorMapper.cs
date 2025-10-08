using System.Net;

namespace CleanResult.WolverineFx;

public static class ErrorMapper
{
    public static Result ToQueryError<T>(this Result<T> result, string notFoundErrorMessage)
    {
        if (result.IsError())
            return Result.From(result);

        if (result.Value is null)
            return Result.Error(notFoundErrorMessage, HttpStatusCode.NotFound);

        throw new InvalidOperationException("Result is not an error");
    }
}