using FluentValidation;
using FluentValidation.Results;

namespace CleanResult.FluentValidation.Tests;

public class ValidationResultExtensionsTests
{
    [Fact]
    public void ToResult_WithValidResult_ReturnsOkResult()
    {
        // Arrange
        var validationResult = new ValidationResult();

        // Act
        var result = validationResult.ToResult();

        // Assert
        Assert.True(result.IsOk());
        Assert.False(result.IsError());
    }

    [Fact]
    public void ToResult_WithInvalidResult_ReturnsErrorResult()
    {
        // Arrange
        var failures = new List<ValidationFailure>
        {
            new ValidationFailure("Name", "Name is required"),
            new ValidationFailure("Email", "Email is invalid")
        };
        var validationResult = new ValidationResult(failures);

        // Act
        var result = validationResult.ToResult();

        // Assert
        Assert.False(result.IsOk());
        Assert.True(result.IsError());
        Assert.Equal(400, result.ErrorValue.Status);
        Assert.Equal("Validation Failed", result.ErrorValue.Title);
    }

    [Fact]
    public void ToResult_WithInvalidResult_ContainsAllErrors()
    {
        // Arrange
        var failures = new List<ValidationFailure>
        {
            new ValidationFailure("Name", "Name is required"),
            new ValidationFailure("Email", "Email is invalid")
        };
        var validationResult = new ValidationResult(failures);

        // Act
        var result = validationResult.ToResult();

        // Assert
        Assert.NotNull(result.ErrorValue.Errors);
        Assert.Equal(2, result.ErrorValue.Errors.Count);
        Assert.Contains("Name", result.ErrorValue.Errors.Keys);
        Assert.Contains("Email", result.ErrorValue.Errors.Keys);
        Assert.Equal(new[] { "Name is required" }, result.ErrorValue.Errors["Name"]);
        Assert.Equal(new[] { "Email is invalid" }, result.ErrorValue.Errors["Email"]);
    }

    [Fact]
    public void ToResult_WithMultipleErrorsForSameProperty_GroupsErrors()
    {
        // Arrange
        var failures = new List<ValidationFailure>
        {
            new ValidationFailure("Email", "Email is required"),
            new ValidationFailure("Email", "Email is invalid format")
        };
        var validationResult = new ValidationResult(failures);

        // Act
        var result = validationResult.ToResult();

        // Assert
        Assert.NotNull(result.ErrorValue.Errors);
        Assert.Single(result.ErrorValue.Errors);
        Assert.Equal(2, result.ErrorValue.Errors["Email"].Length);
        Assert.Contains("Email is required", result.ErrorValue.Errors["Email"]);
        Assert.Contains("Email is invalid format", result.ErrorValue.Errors["Email"]);
    }

    [Fact]
    public void ToResult_WithCustomTitle_UsesCustomTitle()
    {
        // Arrange
        var failures = new List<ValidationFailure>
        {
            new ValidationFailure("Name", "Name is required")
        };
        var validationResult = new ValidationResult(failures);

        // Act
        var result = validationResult.ToResult(title: "Custom Error Title");

        // Assert
        Assert.Equal("Custom Error Title", result.ErrorValue.Title);
    }

    [Fact]
    public void ToResult_WithCustomDetail_UsesCustomDetail()
    {
        // Arrange
        var failures = new List<ValidationFailure>
        {
            new ValidationFailure("Name", "Name is required")
        };
        var validationResult = new ValidationResult(failures);

        // Act
        var result = validationResult.ToResult(detail: "Additional details");

        // Assert
        Assert.Equal("Additional details", result.ErrorValue.Detail);
    }

    [Fact]
    public void ToResult_WithCustomInstance_UsesCustomInstance()
    {
        // Arrange
        var failures = new List<ValidationFailure>
        {
            new ValidationFailure("Name", "Name is required")
        };
        var validationResult = new ValidationResult(failures);

        // Act
        var result = validationResult.ToResult(instance: "/api/users/create");

        // Assert
        Assert.Equal("/api/users/create", result.ErrorValue.Instance);
    }

    [Fact]
    public void ToResultWithValue_WithValidResult_ReturnsOkWithValue()
    {
        // Arrange
        var validationResult = new ValidationResult();
        var value = "test value";

        // Act
        var result = validationResult.ToResult<string>(value);

        // Assert
        Assert.True(result.IsOk());
        Assert.Equal(value, result.Value);
    }

    [Fact]
    public void ToResultWithValue_WithInvalidResult_ReturnsError()
    {
        // Arrange
        var failures = new List<ValidationFailure>
        {
            new ValidationFailure("Name", "Name is required")
        };
        var validationResult = new ValidationResult(failures);
        var value = "test value";

        // Act
        var result = validationResult.ToResult<string>(value);

        // Assert
        Assert.False(result.IsOk());
        Assert.True(result.IsError());
        Assert.Equal(400, result.ErrorValue.Status);
    }

    [Fact]
    public void ToResult_WithRfc9457Type_HasCorrectType()
    {
        // Arrange
        var failures = new List<ValidationFailure>
        {
            new ValidationFailure("Name", "Name is required")
        };
        var validationResult = new ValidationResult(failures);

        // Act
        var result = validationResult.ToResult();

        // Assert
        Assert.Equal("https://tools.ietf.org/html/rfc7231#section-6.5.1", result.ErrorValue.Type);
    }
}
