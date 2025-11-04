# CleanResult.FluentValidation

Integration between CleanResult and FluentValidation for seamless validation error handling.

## Installation

```bash
dotnet add package CleanResult.FluentValidation
```

## Features

- Convert `ValidationResult` to `Result` or `Result<T>`
- Direct validator integration with Result types
- Async validation support
- Automatic conversion of validation errors to RFC 9457 Problem Details format

## Usage

### Basic ValidationResult Conversion

```csharp
using CleanResult.FluentValidation;
using FluentValidation;

public class CreateUserCommand
{
    public string Name { get; set; }
    public string Email { get; set; }
}

public class CreateUserValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Email).EmailAddress();
    }
}

// Convert ValidationResult to Result
var validator = new CreateUserValidator();
var command = new CreateUserCommand { Name = "", Email = "invalid" };
var validationResult = validator.Validate(command);

Result result = validationResult.ToResult();
if (result.IsError())
{
    // Returns 400 Bad Request with validation errors in RFC 9457 format
    return result;
}
```

### Direct Validator Integration

```csharp
// Validate and return Result directly
Result result = validator.ValidateToResult(command);

// Validate and return the validated instance on success
Result<CreateUserCommand> resultWithValue = validator.ValidateToResultWithValue(command);

if (resultWithValue.IsOk())
{
    var validatedCommand = resultWithValue.Value;
    // Use the validated command
}
```

### Async Validation

```csharp
// Async validation
Result result = await validator.ValidateToResultAsync(command);

// Async validation with value
Result<CreateUserCommand> resultWithValue =
    await validator.ValidateToResultWithValueAsync(command);
```

### ASP.NET Core Integration

Use directly in controllers:

```csharp
[HttpPost]
public async Task<Result<User>> CreateUser(
    [FromBody] CreateUserCommand command,
    [FromServices] IValidator<CreateUserCommand> validator)
{
    var validationResult = await validator.ValidateToResultWithValueAsync(command);
    if (validationResult.IsError())
        return validationResult;

    var user = await _userService.CreateUser(validationResult.Value);
    return Result.Ok(user);
}
```

### Customizing Error Messages

```csharp
Result result = validationResult.ToResult(
    title: "User Creation Failed",
    detail: "The provided user data is invalid",
    instance: "/users/create"
);
```

## Error Format

Validation errors are automatically converted to RFC 9457 Problem Details format:

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Validation Failed",
  "status": 400,
  "errors": {
    "Name": ["'Name' must not be empty."],
    "Email": ["'Email' is not a valid email address."]
  }
}
```

## API Reference

### ValidationResultExtensions

- `ToResult()` - Convert ValidationResult to Result
- `ToResult<T>(T value)` - Convert ValidationResult to Result&lt;T&gt; with value

### ValidatorExtensions

- `ValidateToResult<T>(instance)` - Validate and return Result
- `ValidateToResultAsync<T>(instance, cancellationToken)` - Async validate and return Result
- `ValidateToResultWithValue<T>(instance)` - Validate and return Result&lt;T&gt; with validated instance
- `ValidateToResultWithValueAsync<T>(instance, cancellationToken)` - Async version of above

All methods support optional parameters for customizing error details:
- `title` - Error title (default: "Validation Failed")
- `detail` - Optional error detail message
- `instanceUri` / `instance` - Optional URI identifying the error instance

## License

MIT
