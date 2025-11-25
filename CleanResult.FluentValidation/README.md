# CleanResult.FluentValidation

![CleanResult](https://raw.githubusercontent.com/Gwynbleid85/CleanResult/refs/heads/master/docs/readme-header.png)

<div align="center">

[![NuGet](https://img.shields.io/nuget/v/CleanResult.FluentValidation.svg)](https://www.nuget.org/packages/CleanResult.FluentValidation/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-8.0%20%7C%209.0%20%7C%2010.0-purple.svg)](https://dotnet.microsoft.com/download)

[![Build](https://img.shields.io/github/actions/workflow/status/Gwynbleid85/CleanResult/publish.yml?logo=github&label=build)](https://github.com/Gwynbleid85/CleanResult/actions/workflows/publish.yml)
[![Testing](https://img.shields.io/github/actions/workflow/status/Gwynbleid85/CleanResult/publish.yml?logo=github&label=testing)](https://github.com/Gwynbleid85/CleanResult/actions/workflows/publish.yml)

**FluentValidation integration for CleanResult**

Seamlessly convert FluentValidation results to CleanResult types with automatic RFC 9457 compliance

[Main Documentation](../README.md) • [Features](#-features) • [Usage](#-usage) • [API Reference](#-api-reference)

</div>

---

## 📦 Installation

```bash
dotnet add package CleanResult.FluentValidation
```

**Requirements:**

- .NET 8.0, 9.0, or 10.0
- FluentValidation 11.9.0+

---

## ✨ Features

- ✅ **Direct Conversion** - Convert `ValidationResult` to `Result` or `Result<T>`
- 🔄 **Validator Integration** - Call validators that return Result types directly
- ⚡ **Async Support** - Full async/await support with cancellation tokens
- 🎯 **Transform Operations** - Validate and transform to different types in one step
- 📜 **RFC 9457 Compliance** - Automatic conversion to Problem Details format
- 🏷️ **Grouped Errors** - Multiple validation errors per property grouped automatically

---

## 🚀 Usage

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
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(x => x.Email).EmailAddress().WithMessage("Invalid email format");
    }
}

// 🔍 Validate using FluentValidation
var validator = new CreateUserValidator();
var command = new CreateUserCommand { Name = "", Email = "invalid" };
var validationResult = validator.Validate(command);

// ✅ Convert ValidationResult to Result
Result result = validationResult.ToResult();
if (result.IsError())
{
    // Returns 400 Bad Request with RFC 9457 formatted errors
    return result;
}
```

### Direct Validator Integration

```csharp
// ✅ Validate and return Result
Result result = validator.ValidateToResult(command);

// ✅ Validate and return Result<T> with validated instance
Result<CreateUserCommand> resultWithValue = validator.ValidateToResultWithValue(command);

if (resultWithValue.IsOk())
{
    var validatedCommand = resultWithValue.Value;
    // Use the validated command safely
}
```

### Async Validation

```csharp
// ✅ Async validation returning Result
Result result = await validator.ValidateToResultAsync(command);

// ✅ Async validation returning Result<T>
Result<CreateUserCommand> resultWithValue =
    await validator.ValidateToResultWithValueAsync(command);
```

### Validate and Transform

Transform the validated object to another type on success:

```csharp
public class UserDto
{
    public string Name { get; set; }
    public string Email { get; set; }
}

// 🔄 Synchronous transformation
Result<UserDto> dtoResult = validator.ValidateAndTransform(
    command,
    cmd => new UserDto { Name = cmd.Name, Email = cmd.Email }
);

// 🔄 Async transformation
Result<UserDto> dtoResult = await validator.ValidateAndTransformAsync(
    command,
    async cmd => await _mapper.MapAsync(cmd)
);
```

### ASP.NET Core Integration

```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IValidator<CreateUserCommand> _validator;
    private readonly IUserService _userService;

    [HttpPost]
    public async Task<Result<UserDto>> CreateUser([FromBody] CreateUserCommand command)
    {
        // ✅ Validate and transform in one step
        return await _validator.ValidateAndTransformAsync(
            command,
            async validated => await _userService.CreateUser(validated)
        );
    }

    [HttpPut("{id}")]
    public async Task<Result<User>> UpdateUser(
        int id,
        [FromBody] UpdateUserCommand command)
    {
        // ✅ Validate first, then process
        var validationResult = await _validator.ValidateToResultWithValueAsync(command);
        if (validationResult.IsError())
            return validationResult;

        return await _userService.UpdateUser(id, validationResult.Value);
    }
}
```

### Customizing Error Messages

```csharp
Result result = validationResult.ToResult(
    title: "User Creation Failed",
    detail: "The provided user data contains validation errors",
    instance: "/api/users/create"
);
```

---

## 📄 Error Format

Validation errors are automatically converted to **RFC 9457 Problem Details** format:

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Validation Failed",
  "status": 400,
  "detail": null,
  "instance": null,
  "errors": {
    "Name": ["Name is required"],
    "Email": ["Invalid email format"]
  }
}
```

### Multiple Errors Per Property

```csharp
RuleFor(x => x.Email)
    .NotEmpty().WithMessage("Email is required")
    .EmailAddress().WithMessage("Email must be valid");

// Results in:
{
  "errors": {
    "Email": [
      "Email is required",
      "Email must be valid"
    ]
  }
}
```

---

## 📖 API Reference

### ValidationResultExtensions

Extension methods for FluentValidation's `ValidationResult`:

| Method                                               | Description                       |
|------------------------------------------------------|-----------------------------------|
| `ToResult(title, detail, instance)`                  | Convert to `Result` (non-generic) |
| `ToResult<T>(value, title, detail, instance)`        | Convert to `Result<T>` with value |
| `ToResult<T>(valueFactory, title, detail, instance)` | Convert with lazy value creation  |

**Parameters:**

- `title` - Error title (default: "Validation Failed")
- `detail` - Optional detailed error description
- `instance` - Optional URI identifying the error occurrence

### ValidatorExtensions

Extension methods for `IValidator<T>`:

| Method                                                                      | Returns                 | Description                  |
|-----------------------------------------------------------------------------|-------------------------|------------------------------|
| `ValidateToResult<T>(instance, ...)`                                        | `Result`                | Validate and return Result   |
| `ValidateToResultAsync<T>(instance, ...)`                                   | `Task<Result>`          | Async validation             |
| `ValidateToResultWithValue<T>(instance, ...)`                               | `Result<T>`             | Validate and return instance |
| `ValidateToResultWithValueAsync<T>(instance, ...)`                          | `Task<Result<T>>`       | Async with instance          |
| `ValidateAndTransform<TInput, TOutput>(instance, transform, ...)`           | `Result<TOutput>`       | Validate and transform       |
| `ValidateAndTransformAsync<TInput, TOutput>(instance, transform, ...)`      | `Task<Result<TOutput>>` | Async transform (sync)       |
| `ValidateAndTransformAsync<TInput, TOutput>(instance, transformAsync, ...)` | `Task<Result<TOutput>>` | Fully async transform        |

**Common Parameters:**

- `instance` - The object to validate
- `title` - Custom error title
- `detail` - Custom error detail
- `instanceUri` / `instance` - URI identifying the error occurrence
- `cancellationToken` - Cancellation token for async operations

---

## 💡 Examples

### Complex Validation Scenario

```csharp
public class OrderValidator : AbstractValidator<CreateOrderCommand>
{
    public OrderValidator()
    {
        RuleFor(x => x.CustomerId).GreaterThan(0);
        RuleFor(x => x.Items).NotEmpty();
        RuleForEach(x => x.Items).SetValidator(new OrderItemValidator());
    }
}

[HttpPost("orders")]
public async Task<Result<OrderDto>> CreateOrder([FromBody] CreateOrderCommand command)
{
    // Validate and transform in one operation
    return await _orderValidator.ValidateAndTransformAsync(
        command,
        async validated =>
        {
            var order = await _orderService.CreateOrder(validated);
            return _mapper.Map<OrderDto>(order);
        },
        title: "Order Creation Failed",
        detail: "Please check the validation errors and try again"
    );
}
```

### Conditional Validation

```csharp
public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(x => x.Email).EmailAddress();

        When(x => x.IsActive, () =>
        {
            RuleFor(x => x.LastLoginDate).NotNull();
        });
    }
}

var result = await _validator.ValidateToResultWithValueAsync(user);
if (result.IsOk())
{
    await _userRepository.SaveAsync(result.Value);
}
```

---

## 🎯 Best Practices

### ✅ Do's

```csharp
// ✅ Use async methods in async contexts
await validator.ValidateToResultAsync(command);

// ✅ Validate before processing
var validationResult = await validator.ValidateToResultWithValueAsync(dto);
if (validationResult.IsOk())
{
    await ProcessValidatedData(validationResult.Value);
}

// ✅ Use transform for mapping operations
return await validator.ValidateAndTransformAsync(
    command,
    async cmd => await _service.Execute(cmd)
);
```

### ❌ Don'ts

```csharp
// ❌ Don't ignore validation results
validator.ValidateToResult(command);  // Result ignored!
await ProcessData(command);

// ❌ Don't access Value without checking
var result = validator.ValidateToResultWithValue(command);
var name = result.Value.Name;  // Throws if validation failed!

// ❌ Don't validate synchronously in async methods
public async Task<Result> CreateAsync(CreateCommand command)
{
    var result = validator.ValidateToResult(command);  // Should use ValidateToResultAsync
    // ...
}
```

---

## 🔗 Related Packages

- **[CleanResult](../README.md)** - Core Result implementation
- **[CleanResult.WolverineFx](../CleanResult.WolverineFx/README.md)** - WolverineFx messaging integration
- **[CleanResult.Swashbuckle](../CleanResult.Swashbuckle/README.md)** - Swagger/OpenAPI integration
- **[CleanResult.AspNet](../CleanResult.AspNet/README.md)** - IActionResult adapter

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](../LICENSE) file for details.

---

<div align="center">

**[⬆ Back to Top](#cleanresultfluentvalidation)** • **[Main Documentation](../README.md)**

</div>

<div align="center">
Gwynbleid85 © 2025
</div>