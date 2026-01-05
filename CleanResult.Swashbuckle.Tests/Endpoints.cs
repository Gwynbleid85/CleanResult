using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;

namespace CleanResult.Swashbuckle.Tests;

/// <summary>
/// Custom response model for testing.
/// </summary>
/// <param name="Id">The identifier.</param>
/// <param name="Name">The name.</param>
public record CustomResponse(int? Id, string Name);

/// <summary>
/// Test endpoints for CleanResult Swashbuckle integration.
/// </summary>
public class Endpoints
{
    /// <summary>
    /// Returns a simple integer result.
    /// </summary>
    /// <returns>A Result containing an integer value.</returns>
    [ProducesResponseType<Error>(StatusCodes.Status404NotFound)]
    [WolverineGet("/")]
    public static Result<int> Index()
    {
        return Result.Ok(1);
    }

    /// <summary>
    /// Returns a custom response object.
    /// </summary>
    /// <returns>A Result containing a CustomResponse object.</returns>
    [ProducesResponseType<Error>(StatusCodes.Status404NotFound)]
    [WolverineGet("/custom-response")]
    public static Result<CustomResponse> Index2()
    {
        return Result.Ok(new CustomResponse(1, "Custom Response"));
    }

    /// <summary>
    /// Returns an error result.
    /// </summary>
    /// <returns>A Result with an error.</returns>
    [ProducesResponseType<Error>(StatusCodes.Status404NotFound)]
    [WolverineGet("/error")]
    public static Result<int> Index3()
    {
        return Result.Error("Not found", 404);
    }

    /// <summary>
    /// Returns an error result with a custom error code.
    /// </summary>
    /// <returns>A Result with a custom error code.</returns>
    [ProducesResponseType<Error>(StatusCodes.Status404NotFound)]
    [WolverineGet("/custom-error-code")]
    public static Result<int> Index4()
    {
        return Result.Error("Custom error code", 1234);
    }


    /// <summary>
    /// Returns a nullable custom response.
    /// </summary>
    /// <returns></returns>
    [WolverineGet("/nullable-test")]
    public static Result<CustomResponse?> Index5()
    {
        return Result.Ok<CustomResponse?>(null);
    }

    [WolverineGet("/nullable-test2")]
    public static Result<NullableResponse> Index6()
    {
        return new Result<NullableResponse>();
    }

    public class NullableResponse
    {
        public required CustomResponse Data1 { get; set; }
        public CustomResponse Data2 { get; set; }
        public required CustomResponse? Data3 { get; set; }
        public CustomResponse? Data4 { get; set; }
    }
}