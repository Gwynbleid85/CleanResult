using System.Net;
using CleanResult;
using CleanResult.WolverineFx;

namespace WolverineTests;

public class ToQueryErrorTests
{
    [Fact]
    public void ToQueryError_ShouldReturnNotFoundError_WhenResultIsNull()
    {
        // Arrange
        var result = Result<string?>.Ok(null);
        var notFoundErrorMessage = "Resource not found";

        // Act
        var queryError = result.ToQueryError(notFoundErrorMessage);

        // Assert
        Assert.False(queryError.IsOk());
        Assert.Equal(notFoundErrorMessage, queryError.ErrorValue.Title);
        Assert.Equal(404, queryError.ErrorValue.Status);
    }

    [Fact]
    public void ToQueryError_ShouldReturnOriginalError_WhenResultIsError()
    {
        // Arrange
        var originalError = Result<string>.Error("Original error", HttpStatusCode.BadRequest);
        var notFoundErrorMessage = "Resource not found";

        // Act
        var queryError = originalError.ToQueryError(notFoundErrorMessage);

        // Assert
        Assert.True(queryError.IsError());
        Assert.Equal(originalError.ErrorValue.Title, queryError.ErrorValue.Title);
        Assert.Equal(originalError.ErrorValue.Status, queryError.ErrorValue.Status);
    }

    [Fact]
    public void ToQueryError_ShouldThrowInvalidOperationException_WhenResultIsOkWithValue()
    {
        // Arrange
        var result = Result<string>.Ok("Valid value");
        var notFoundErrorMessage = "Resource not found";

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => result.ToQueryError(notFoundErrorMessage));
        Assert.Equal("Result is not an error", exception.Message);
    }
}