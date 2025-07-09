using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CleanResult.AspNetCore;

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
    public async Task ExecuteResultAsync(ActionContext actionContext)
    {
        if (result.IsOk())
        {
            actionContext.HttpContext.Response.StatusCode = StatusCodes.Status204NoContent;
            return;
        }

        // Error
        actionContext.HttpContext.Response.StatusCode = result.ErrorValue.Code;
        actionContext.HttpContext.Response.ContentType = "application/json";
        await actionContext.HttpContext.Response.WriteAsync(JsonSerializer.Serialize(new
        {
            result.ErrorValue.Message, result.ErrorValue.Code
        }, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}

public class AspActionResult<T>(Result<T> result) : IActionResult
{
    public async Task ExecuteResultAsync(ActionContext actionContext)
    {
        if (result.IsOk())
        {
            actionContext.HttpContext.Response.StatusCode = StatusCodes.Status200OK;
            actionContext.HttpContext.Response.ContentType = "application/json";
            await actionContext.HttpContext.Response.WriteAsync(JsonSerializer.Serialize(result.Value,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }));
            return;
        }

        // Error
        actionContext.HttpContext.Response.StatusCode = result.ErrorValue.Code;
        actionContext.HttpContext.Response.ContentType = "application/json";
        await actionContext.HttpContext.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                result.ErrorValue.Message, result.ErrorValue.Code
            }, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }
        ));
    }
}