using FluentValidation;

namespace CleanResult.FluentValidation.Tests;

public class ValidatorExtensionsTests
{
    [Fact]
    public void ValidateToResult_WithValidInstance_ReturnsOk()
    {
        // Arrange
        var validator = new TestCommandValidator();
        var command = new TestCommand
        {
            Name = "John Doe",
            Email = "john@example.com",
            Age = 30
        };

        // Act
        var result = validator.ValidateToResult(command);

        // Assert
        Assert.True(result.IsOk());
    }

    [Fact]
    public void ValidateToResult_WithInvalidInstance_ReturnsError()
    {
        // Arrange
        var validator = new TestCommandValidator();
        var command = new TestCommand
        {
            Name = "",
            Email = "invalid-email",
            Age = -5
        };

        // Act
        var result = validator.ValidateToResult(command);

        // Assert
        Assert.False(result.IsOk());
        Assert.True(result.IsError());
        Assert.Equal(400, result.ErrorValue.Status);
    }

    [Fact]
    public async Task ValidateToResultAsync_WithValidInstance_ReturnsOk()
    {
        // Arrange
        var validator = new TestCommandValidator();
        var command = new TestCommand
        {
            Name = "John Doe",
            Email = "john@example.com",
            Age = 30
        };

        // Act
        var result = await validator.ValidateToResultAsync(command);

        // Assert
        Assert.True(result.IsOk());
    }

    [Fact]
    public async Task ValidateToResultAsync_WithInvalidInstance_ReturnsError()
    {
        // Arrange
        var validator = new TestCommandValidator();
        var command = new TestCommand
        {
            Name = "",
            Email = "invalid-email",
            Age = -5
        };

        // Act
        var result = await validator.ValidateToResultAsync(command);

        // Assert
        Assert.False(result.IsOk());
        Assert.True(result.IsError());
    }

    [Fact]
    public void ValidateToResultWithValue_WithValidInstance_ReturnsOkWithValue()
    {
        // Arrange
        var validator = new TestCommandValidator();
        var command = new TestCommand
        {
            Name = "John Doe",
            Email = "john@example.com",
            Age = 30
        };

        // Act
        var result = validator.ValidateToResultWithValue(command);

        // Assert
        Assert.True(result.IsOk());
        Assert.Equal(command, result.Value);
        Assert.Equal("John Doe", result.Value.Name);
    }

    [Fact]
    public void ValidateToResultWithValue_WithInvalidInstance_ReturnsError()
    {
        // Arrange
        var validator = new TestCommandValidator();
        var command = new TestCommand
        {
            Name = "",
            Email = "invalid-email",
            Age = -5
        };

        // Act
        var result = validator.ValidateToResultWithValue(command);

        // Assert
        Assert.False(result.IsOk());
        Assert.True(result.IsError());
        Assert.NotNull(result.ErrorValue.Errors);
        Assert.Equal(3, result.ErrorValue.Errors.Count);
    }

    [Fact]
    public async Task ValidateToResultWithValueAsync_WithValidInstance_ReturnsOkWithValue()
    {
        // Arrange
        var validator = new TestCommandValidator();
        var command = new TestCommand
        {
            Name = "John Doe",
            Email = "john@example.com",
            Age = 30
        };

        // Act
        var result = await validator.ValidateToResultWithValueAsync(command);

        // Assert
        Assert.True(result.IsOk());
        Assert.Equal(command, result.Value);
    }

    [Fact]
    public async Task ValidateToResultWithValueAsync_WithInvalidInstance_ReturnsError()
    {
        // Arrange
        var validator = new TestCommandValidator();
        var command = new TestCommand
        {
            Name = "",
            Email = "invalid-email",
            Age = -5
        };

        // Act
        var result = await validator.ValidateToResultWithValueAsync(command);

        // Assert
        Assert.False(result.IsOk());
        Assert.True(result.IsError());
    }

    [Fact]
    public void ValidateToResult_WithCustomTitle_UsesCustomTitle()
    {
        // Arrange
        var validator = new TestCommandValidator();
        var command = new TestCommand
        {
            Name = "",
            Email = "invalid",
            Age = -1
        };

        // Act
        var result = validator.ValidateToResult(command, "Custom Validation Error");

        // Assert
        Assert.True(result.IsError());
        Assert.Equal("Custom Validation Error", result.ErrorValue.Title);
    }

    [Fact]
    public void ValidateToResult_WithCustomDetailAndInstance_UsesCustomValues()
    {
        // Arrange
        var validator = new TestCommandValidator();
        var command = new TestCommand
        {
            Name = "",
            Email = "invalid",
            Age = -1
        };

        // Act
        var result = validator.ValidateToResult(
            command,
            "Validation Error",
            "The command contains invalid data",
            "/api/commands/123");

        // Assert
        Assert.True(result.IsError());
        Assert.Equal("The command contains invalid data", result.ErrorValue.Detail);
        Assert.Equal("/api/commands/123", result.ErrorValue.Instance);
    }

    [Fact]
    public async Task ValidateToResultAsync_WithCancellation_SupportsCancellation()
    {
        // Arrange
        var validator = new TestCommandValidator();
        var command = new TestCommand
        {
            Name = "John Doe",
            Email = "john@example.com",
            Age = 30
        };
        var cts = new CancellationTokenSource();

        // Act
        var result = await validator.ValidateToResultAsync(command, cancellationToken: cts.Token);

        // Assert
        Assert.True(result.IsOk());
    }

    public class TestCommand
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Age { get; set; }
    }

    public class TestCommandValidator : AbstractValidator<TestCommand>
    {
        public TestCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
            RuleFor(x => x.Email).EmailAddress().WithMessage("Email must be valid");
            RuleFor(x => x.Age).GreaterThan(0).WithMessage("Age must be positive");
        }
    }

    public class TestDto
    {
        public string FullName { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
    }
}