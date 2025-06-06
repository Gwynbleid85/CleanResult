using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace CleanResult;

public partial class Result : IResult
{
    public async Task ExecuteAsync(HttpContext httpContext)
    {
        if (Success)
        {
            httpContext.Response.StatusCode = StatusCodes.Status204NoContent;
            return;
        }

        // Error
        httpContext.Response.StatusCode = ErrorCode ?? StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/json";
        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(new Error
        {
            Message = ErrorMessage ?? string.Empty,
            Code = ErrorCode ?? StatusCodes.Status500InternalServerError
        }, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}

public partial class Result<T> : IResult
{
    public async Task ExecuteAsync(HttpContext httpContext)
    {
        if (Success)
        {
            httpContext.Response.StatusCode = StatusCodes.Status200OK;
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsync(JsonSerializer.Serialize(SuccessValue,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }));
            return;
        }

        // Error
        httpContext.Response.StatusCode = ErrorCode ?? StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/json";
        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(new Error
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