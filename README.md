# CleanResult

![CleanResult](https://github.com/Gwynbleid85/CleanResult/blob/master/docs/readme-header.png?raw=true)

<div align="center">

[![NuGet](https://img.shields.io/nuget/v/CleanResult.svg)](https://www.nuget.org/packages/CleanResult/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/download)

**A clean, Rust-inspired Result type implementation for .NET 8.0**

Brings functional error handling to C# with built-in ASP.NET Core integration and RFC 9457 compliance

[Features](#-features) • [Quick Start](#-quick-start) • [Extensions](#-extension-packages) • [Examples](#-examples) • [Documentation](#-documentation)

</div>

---

## ✨ Features

### Core Capabilities

- 🦀 **Rust-inspired Design** - Familiar `Result<T>` pattern for functional error handling
- 🔒 **Type Safety** - Compile-time guarantees for error handling paths
- 🔄 **Automatic Type Conversions** - Implicit conversions between `Result<T>` and `Result` for seamless error
  propagation
- 🌐 **ASP.NET Core Integration** - Direct `IResult` implementation for seamless web API integration
- 📜 **RFC 9457 Compliant** - Full Problem Details specification support
- 🎯 **Smart Content-Type Detection** - Automatically sets correct Content-Type based on return type
- 🪶 **Zero Dependencies** - Lightweight library with minimal overhead
- ⚡ **High Performance** - No reflection, optimized for speed

### Content-Type Intelligence

CleanResult automatically selects the appropriate Content-Type based on your return value:

| Type                    | Content-Type                      | Example              |
|-------------------------|-----------------------------------|----------------------|
| `string`                | `text/plain; charset=utf-8`       | Plain text responses |
| `byte[]`                | `application/octet-stream`        | Binary data, files   |
| `Stream`                | `application/octet-stream`        | File streams         |
| `XDocument`, `XElement` | `application/xml; charset=utf-8`  | XML documents        |
| Objects, primitives     | `application/json; charset=utf-8` | JSON responses       |

---

## 📦 Extension Packages

CleanResult offers specialized extensions for popular frameworks:

| Package                                                           | Description                       | Documentation                                  |
|-------------------------------------------------------------------|-----------------------------------|------------------------------------------------|
| **[CleanResult.FluentValidation](CleanResult.FluentValidation/)** | FluentValidation integration      | [Docs](CleanResult.FluentValidation/README.md) |
| **[CleanResult.WolverineFx](CleanResult.WolverineFx/)**           | WolverineFx messaging integration | [Docs](CleanResult.WolverineFx/README.md)      |
| **[CleanResult.Swashbuckle](CleanResult.Swashbuckle/)**           | Swagger/OpenAPI integration       | [Docs](CleanResult.Swashbuckle/README.md)      |
| **[CleanResult.AspNet](CleanResult.AspNet/)**                     | Traditional IActionResult adapter | [Docs](CleanResult.AspNet/README.md)           |

---

## 🚀 Quick Start

### Installation

```bash
dotnet add package CleanResult
```

### Basic Usage

```csharp
using CleanResult;

// ✅ Success without value
Result simpleSuccess = Result.Ok();

// ✅ Success with value
Result<User> userResult = Result.Ok(new User { Name = "John" });

// ❌ Error with message and HTTP status
Result error = Result.Error("User not found", 404);
```

### ASP.NET Core Controller

```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : Controller
{
    [HttpGet("{id}")]
    public Result<User> GetUser(int id)
    {
        var user = _userService.FindById(id);

        if (user == null)
            return Result.Error("User not found", 404);

        return Result.Ok(user);  // Returns HTTP 200 with JSON
    }

    [HttpPost]
    public Result CreateUser([FromBody] User user)
    {
        if (string.IsNullOrEmpty(user.Email))
            return Result.Error("Email is required", 400);

        _userService.Create(user);
        return Result.Ok();  // Returns HTTP 204 No Content
    }
}
```

### Content-Type Examples

```csharp
// 📄 Plain text
[HttpGet("message")]
public Result<string> GetMessage()
    => Result.Ok("Hello World");  // Content-Type: text/plain

// 📦 Binary data
[HttpGet("file")]
public Result<byte[]> GetFile()
    => Result.Ok(fileBytes);  // Content-Type: application/octet-stream

// 🔤 XML document
[HttpGet("xml")]
public Result<XDocument> GetXml()
    => Result.Ok(xmlDoc);  // Content-Type: application/xml

// 📊 JSON object (default)
[HttpGet("data")]
public Result<User> GetData()
    => Result.Ok(user);  // Content-Type: application/json
```

---

## 📚 Examples

### Service Layer Pattern

```csharp
public class UserService
{
    private readonly IUserRepository _repository;

    public Result<User> GetById(int id)
    {
        var user = _repository.FindById(id);

        if (user == null)
            return Resul.Error("User not found", HttpStatusCode.NotFound);

        return Result.Ok(user);
    }

    public Result<User> Create(CreateUserDto dto)
    {
        // Validation
        if (string.IsNullOrEmpty(dto.Email))
            return Result.Error("Email is required", HttpStatusCode.BadRequest);

        if (_repository.ExistsByEmail(dto.Email))
            return Result.Error("Email already exists", HttpStatusCode.Conflict);

        // Create user
        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email
        };

        _repository.Add(user);
        return Result.Ok(user);
    }
}
```

### Error Chaining and Conversion

```csharp
public class OrderService
{
    public Result ProcessOrder(int userId, int productId, int quantity)
    {
        // Validate user
        var userResult = _userService.GetById(userId);
        if (userResult.IsError())
            return userResult;  // Implicit conversion from Result<User> to Result

        // Validate product
        var productResult = _productService.GetById(productId);
        if (productResult.IsError())
            return productResult;  // Implicit conversion from Result<Product> to Result

        // Process order with validated entities
        var user = userResult.Value;
        var product = productResult.Value;

        if (product.Stock < quantity)
            return Result.Error("Insufficient stock", HttpStatusCode.Conflict);

        // Create order...
        return Result.Ok();
    }
}
```

### Exception Wrapping

```csharp
public class FileService
{
    public Result<string> ReadFile(string path)
    {
        try
        {
            if (!File.Exists(path))
                return Result.Error("File not found", 404);

            var content = File.ReadAllText(path);
            return Result.Ok(content);
        }
        catch (UnauthorizedAccessException)
        {
            return Result.Error("Access denied", 403);
        }
        catch (Exception ex)
        {
            return Result.Error(
                $"Failed to read file: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }
}
```

---

## 📖 Documentation

### API Reference

#### Result (Non-Generic)

```csharp
// Factory Methods
Result.Ok()                                          // Success without value
Result.Error(string title)                           // Error with title
Result.Error(string title, int status)               // Error with status code
Result.Error(string title, HttpStatusCode status)    // Error with HTTP status
Result.Error(Error error)                            // Error from Error object

// Conversion
Result.From<T>(Result<T> result)                     // Convert from generic result

// State Checking
bool IsOk()                                          // Check if success
bool IsError()                                       // Check if error
Error ErrorValue                                     // Get error (throws if Ok)
```

#### Result<T> (Generic)

```csharp
// Factory Methods
Result<T>.Ok(T value)                                // Success with value
Result<T>.Error(string title)                        // Error with title
Result<T>.Error(string title, int status)            // Error with status code
Result<T>.Error(string title, HttpStatusCode status) // Error with HTTP status
Result<T>.Error(Error error)                         // Error from Error object

// Conversion
Result<T>.From<T1>(Result<T1> result)                // Convert between types (errors only)

// State Checking
bool IsOk()                                          // Check if success
bool IsError()                                       // Check if error
T Value                                              // Get value (throws if Error)
Error ErrorValue                                     // Get error (throws if Ok)
```

#### Error Structure (RFC 9457 Compliant)

```csharp
public struct Error
{
    public string Type { get; set; }                    // URI identifying problem type
    public string Title { get; set; }                   // Human-readable summary
    public int Status { get; set; }                     // HTTP status code
    public string? Detail { get; set; }                 // Specific explanation
    public string? Instance { get; set; }               // URI identifying occurrence
    public IDictionary<string, string[]>? Errors { get; set; }  // Validation errors
}
```

### HTTP Response Behavior

| Result                      | HTTP Status    | Content-Type             | Body            |
|-----------------------------|----------------|--------------------------|-----------------|
| `Result.Ok()`               | 204 No Content | application/json         | Empty           |
| `Result<string>.Ok("text")` | 200 OK         | text/plain               | Raw string      |
| `Result<User>.Ok(user)`     | 200 OK         | application/json         | JSON object     |
| `Result<byte[]>.Ok(data)`   | 200 OK         | application/octet-stream | Binary data     |
| `Result.Error("msg", 404)`  | 404 Not Found  | application/json         | Problem Details |

**Error Response (RFC 9457):**

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "User not found",
  "status": 404,
  "detail": "No user exists with ID 123",
  "instance": "/api/users/123"
}
```

---

## 🎯 Best Practices

### ✅ Do's

```csharp
// ✅ Use Result for expected error conditions
public Result<User> GetUser(int id)
{
    var user = _repository.FindById(id);
    return user == null
        ? Result.Error("Not found", 404)
        : Result.Ok(user);
}

// ✅ Check results before accessing values
var result = GetUser(123);
if (result.IsOk())
{
    Console.WriteLine(result.Value.Name);
}

// ✅ Use HTTP status codes for web APIs
return Result.Error("Unauthorized", HttpStatusCode.Unauthorized);
```

### ❌ Don'ts

```csharp
// ❌ Don't use Result for unexpected exceptions
public Result<string> ReadFile(string path)
{
    // Bad: Let exceptions propagate for unexpected errors
    var content = File.ReadAllText(path);  // Could throw unexpected IOException
    return Result.Ok(content);
}

// ❌ Don't access Value without checking
var result = GetUser(123);
var name = result.Value.Name;  // Throws if result is error!

// ❌ Don't expose internal error details to users
return Result.Error(ex.StackTrace, 500);  // Bad: leaks implementation details
```

---

## 🔧 Advanced Usage

With extension packages, CleanResult can integrate with popular libraries and frameworks.

### FluentValidation Integration

**[📚 Full Documentation](CleanResult.FluentValidation/README.md)**

```csharp
// Install: dotnet add package CleanResult.FluentValidation

public class CreateUserValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Email).EmailAddress();
        RuleFor(x => x.Name).NotEmpty();
    }
}

[HttpPost]
public async Task<Result<User>> CreateUser(
    [FromBody] CreateUserDto dto,
    [FromServices] IValidator<CreateUserDto> validator)
{
    // Validate and return Result directly
    var validationResult = await validator.ValidateToResultAsync(dto);
    if (validationResult.IsError())
        return Result<User>.From(validationResult);

    // Or validate and transform in one step
    return await validator.ValidateAndTransformAsync(
        dto,
        async validated => await _userService.CreateAsync(validated)
    );
}
```

### WolverineFx Messaging

**[📚 Full Documentation](CleanResult.WolverineFx/README.md)**

```csharp
// Install: dotnet add package CleanResult.WolverineFx

public class CreateUserHandler
{
    // Load returns Result<T> - framework extracts value or short-circuits on error
    public static async Task<Result<User>> LoadAsync(CreateUserCommand command)
    {
        var user = await _repository.FindByEmailAsync(command.Email);
        if (user != null)
            return Result<User>.Error("Email exists", 409);

        return Result.Ok(new User { Email = command.Email });
    }

    // Handle receives extracted value only if Load succeeded
    public static async Task<Result<UserCreatedEvent>> Handle(
        CreateUserCommand command,
        User user)
    {
        await _repository.SaveAsync(user);
        return Result.Ok(new UserCreatedEvent(user.Id));
    }
}
```

### Swagger/OpenAPI Integration

**[📚 Full Documentation](CleanResult.Swashbuckle/README.md)**

```csharp
// Install: dotnet add package CleanResult.Swashbuckle

builder.Services.AddSwaggerGen(options =>
{
    // Unwraps Result<T> types to show actual return types in Swagger UI
    options.AddCleanResultFilters();
});
```

---

## 🤝 Contributing

Contributions are welcome! Please feel free to submit issues, feature requests, or pull requests.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 🙏 Acknowledgments

Inspired by Rust's `Result<T, E>` type and the functional programming community.

---

<div align="center">

**[⬆ Back to Top](#cleanresult)**


</div>

<div align="center">
Gwynbleid85 © 2025
</div>