using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace CleanResult.AspNetCore;

public static class AspResultExtensions
{
    public static IResult ToIResult(this Result result)
    {
        return new AspResult(result);
    }

    public static IResult ToIResult<T>(this Result<T> result)
    {
        return new AspResult<T>(result);
    }
}

public class AspResult(Result result) : IResult
{
    public async Task ExecuteAsync(HttpContext httpContext)
    {
        if (result.IsOk())
        {
            httpContext.Response.StatusCode = StatusCodes.Status204NoContent;
            return;
        }

        // Error
        httpContext.Response.StatusCode = result.ErrorValue.Code;
        httpContext.Response.ContentType = "application/json";
        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(new
        {
            result.ErrorValue.Message, result.ErrorValue.Code
        }, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}

public class AspResult<T>(Result<T> result) : IResult
{
    public async Task ExecuteAsync(HttpContext httpContext)
    {
        if (result.IsOk())
        {
            httpContext.Response.StatusCode = StatusCodes.Status200OK;
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsync(JsonSerializer.Serialize(result.Value,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }));
            return;
        }

        // Error
        httpContext.Response.StatusCode = result.ErrorValue.Code;
        httpContext.Response.ContentType = "application/json";
        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                result.ErrorValue.Message, result.ErrorValue.Code
            }, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }
        ));
    }
}