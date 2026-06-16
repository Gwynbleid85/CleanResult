using Microsoft.AspNetCore.Mvc;

namespace CleanResult.AspNet;

public static class AspActionResultExtensions
{
    public static IActionResult ToIActionResult(this Result result)
    {
        return new AspActionResult(result);
    }

    public static IActionResult ToIActionResult<T>(this Result<T> result)
    {
        return new AspActionResult<T>(result);
    }
}

public class AspActionResult(Result result) : IActionResult
{
    public Task ExecuteResultAsync(ActionContext actionContext)
    {
        return result.ExecuteAsync(actionContext.HttpContext);
    }
}

public class AspActionResult<T>(Result<T> result) : IActionResult
{
    public Task ExecuteResultAsync(ActionContext actionContext)
    {
        return result.ExecuteAsync(actionContext.HttpContext);
    }
}
