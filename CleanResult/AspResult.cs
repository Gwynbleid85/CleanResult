using Microsoft.AspNetCore.Http;

namespace CleanResult;

public static class ResultExtensions
{
    public static IResult ToIResult(this Result result)
    {
        return new AspResult(result.Success, result.ErrorMessage, result.ErrorCode);
    }

    public static IResult ToIResult<T>(this Result<T> result)
    {
        return new AspResult<T>(result.Success, result.SuccessValue, result.ErrorMessage, result.ErrorCode);
    }
}