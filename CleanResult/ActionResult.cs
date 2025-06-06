using Microsoft.AspNetCore.Mvc;

namespace CleanResult;

public partial class Result : IActionResult
{
    public Task ExecuteResultAsync(ActionContext context)
    {
        return ExecuteAsync(context.HttpContext);
    }
}

public partial class Result<T> : IActionResult
{
    public Task ExecuteResultAsync(ActionContext context)
    {
        return ExecuteAsync(context.HttpContext);
    }
}