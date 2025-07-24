using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;

namespace CleanResult.Swashbuckle.Tests;

public record CustomResponse(int? Id, string Name);

public class Endpoints
{
    [ProducesResponseType<Error>(StatusCodes.Status404NotFound)]
    [WolverineGet("/")]
    public static Result<int> Index()
    {
        return Result.Ok(1);
    }

    [ProducesResponseType<Error>(StatusCodes.Status404NotFound)]
    [WolverineGet("/custom-response")]
    public static Result<CustomResponse> Index2()
    {
        return Result.Ok(new CustomResponse(1, "Custom Response"));
    }

    [ProducesResponseType<Error>(StatusCodes.Status404NotFound)]
    [WolverineGet("/error")]
    public static Result<int> Index3()
    {
        return Result.Error("Not found", 404);
    }

    [ProducesResponseType<Error>(StatusCodes.Status404NotFound)]
    [WolverineGet("/custom-error-code")]
    public static Result<int> Index4()
    {
        return Result.Error("Custom error code", 1234);
    }
}