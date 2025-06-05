using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CleanResult;

public partial class Result : IActionResult
{
    public async Task ExecuteResultAsync(ActionContext context)
    {
        if (Success)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status204NoContent;
            return;
        }

        // Error
        context.HttpContext.Response.StatusCode = ErrorCode ?? StatusCodes.Status500InternalServerError;
        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsync(JsonSerializer.Serialize(new Error
        {
            Message = ErrorMessage ?? string.Empty,
            Code = ErrorCode ?? StatusCodes.Status500InternalServerError
        }, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}

public partial class Result<T>: IActionResult
{
    public async Task ExecuteResultAsync(ActionContext context)
    {
        if (Success)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status200OK;
            context.HttpContext.Response.ContentType = "application/json";
            await context.HttpContext.Response.WriteAsync(JsonSerializer.Serialize(SuccessValue,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }));
            return;
        }

        // Error
        context.HttpContext.Response.StatusCode = ErrorCode ?? StatusCodes.Status500InternalServerError;
        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsync(JsonSerializer.Serialize(new Error
            {
                Message = ErrorMessage ?? string.Empty,
                Code = ErrorCode ?? StatusCodes.Status500InternalServerError
            }, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }
        ));
    }
}