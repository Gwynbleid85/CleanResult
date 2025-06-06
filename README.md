# CleanResult
![CleanIAM](docs/readme-header.png)

A clean, Rust-inspired Result type implementation for .NET that brings functional error handling to your C# applications. This library provides a robust alternative to traditional exception-based error handling with built-in ASP.NET Core integration.

## Features

- **Rust-inspired Design**: Familiar Result<T> pattern for developers coming from Rust
- **ASP.NET Core Integration**: Built-in `IActionResult` implementation for seamless web API integration
- **Type Safety**: Compile-time guarantees for error handling
- **Zero Dependencies**: Lightweight library with minimal overhead
- **Fluent API**: Clean, readable syntax for result handling
- **HTTP Status Code Support**: Direct integration with `HttpStatusCode` enum

## Installation

```bash
dotnet add package CleanResult
```

## Quick Start

### Basic Usage

```csharp
using CleanResult;

// Success without value
Result simpleSuccess = Result.Ok();

// Success with value
Result<string> successWithValue = Result.Ok("Hello, World!");

// Error without details
Result simpleError = Result.Error();

// Error with message
Result errorWithMessage = Result.Error("Something went wrong");

// Error with message and code
Result errorWithCode = Result.Error("Not found", 404);

// Error with HTTP status code
Result httpError = Result.Error("Unauthorized", HttpStatusCode.Unauthorized);
```

### Checking Results

```csharp
var result = GetUserById(123);

if (result.IsOk())
{
    var user = result.Value;
    Console.WriteLine($"Found user: {user.Name}");
}
else if (result.IsError())
{
    var error = result.ErrorValue;
    Console.WriteLine($"Error {error.Code}: {error.Message}");
}
```

## Detailed Examples

### Service Layer Implementation

```csharp
public class UserService
{
    private readonly List<User> _users = new();

    public Result<User> GetUserById(int id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        
        if (user == null)
        {
            return Result<User>.Error("User not found", HttpStatusCode.NotFound);
        }
        
        return Result.Ok(user);
    }

    public Result CreateUser(User user)
    {
        if (string.IsNullOrEmpty(user.Email))
        {
            return Result.Error("Email is required", HttpStatusCode.BadRequest);
        }

        if (_users.Any(u => u.Email == user.Email))
        {
            return Result.Error("Email already exists", HttpStatusCode.Conflict);
        }

        _users.Add(user);
        return Result.Ok();
    }

    public Result<User> UpdateUser(int id, User updatedUser)
    {
        var existingUser = _users.FirstOrDefault(u => u.Id == id);
        
        if (existingUser == null)
        {
            return Result<User>.Error("User not found", HttpStatusCode.NotFound);
        }

        existingUser.Name = updatedUser.Name;
        existingUser.Email = updatedUser.Email;
        
        return Result.Ok(existingUser);
    }
}
```

### ASP.NET Core Controller Integration

```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;

    public UsersController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{id}")]
    public Result<User> GetUser(int id)
    {
        // Direct return - Result<T> implements IActionResult
        return _userService.GetUserById(id);
    }

    [HttpPost]
    public Result CreateUser([FromBody] User user)
    {
        // Direct return - Result implements IActionResult
        return _userService.CreateUser(user);
    }

    [HttpPut("{id}")]
    public Result<User> UpdateUser(int id, [FromBody] User user)
    {
        return _userService.UpdateUser(id, user);
    }
}
```

### Error Handling Patterns

```csharp
public class FileService
{
    public Result<string> ReadFile(string path)
    {
        try
        {
            if (!File.Exists(path))
            {
                return Result<string>.Error("File not found", HttpStatusCode.NotFound);
            }

            var content = File.ReadAllText(path);
            return Result.Ok(content);
        }
        catch (UnauthorizedAccessException)
        {
            return Result<string>.Error("Access denied", HttpStatusCode.Forbidden);
        }
        catch (Exception ex)
        {
            return Result<string>.Error($"Failed to read file: {ex.Message}", 
                HttpStatusCode.InternalServerError);
        }
    }
}
```

### Result Conversion and Chaining

```csharp
public class OrderService
{
    private readonly UserService _userService;
    private readonly ProductService _productService;

    public Result ProcessOrder(int userId, int productId, int quantity)
    {
        // Chain operations - convert Result<T> to Result when needed
        var userResult = _userService.GetUserById(userId);
        if (userResult.IsError())
            return userResult; // Use auto conversion of error in type Result<User> to Result

        var productResult = _productService.GetProductById(productId);
        if (productResult.IsError())
            return userResult; // Use auto conversion of error in type Result<User> to Result

        // Process the order...
        return Result.Ok();
    }
}
```

### Advanced Error Handling

```csharp
public class PaymentService
{
    public Result<PaymentResult> ProcessPayment(decimal amount, string cardToken)
    {
        if (amount <= 0)
        {
            return Result.Error(
                "Invalid amount", 
                HttpStatusCode.BadRequest
            );
        }

        if (string.IsNullOrEmpty(cardToken))
        {
            return Result.Error(
                "Card token is required", 
                HttpStatusCode.BadRequest
            );
        }

        try
        {
            // Simulate payment processing
            var paymentResult = ProcessWithProvider(amount, cardToken);
            
            if (paymentResult.Success)
            {
                return Result.Ok(paymentResult);
            }
            
            return Result.Error(
                paymentResult.ErrorMessage ?? "Payment failed",
                HttpStatusCode.PaymentRequired
            );
        }
        catch (Exception ex)
        {
            return Result.Error(
                $"Payment processing error: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }
}
```

## API Reference

### Result (Non-Generic)

| Method | Description |
|--------|-------------|
| `Result.Ok()` | Creates a successful result without a value |
| `Result.Error()` | Creates an error result without details |
| `Result.Error(string message)` | Creates an error result with a message |
| `Result.Error(string message, int code)` | Creates an error result with message and code |
| `Result.Error(string message, HttpStatusCode code)` | Creates an error result with message and HTTP status code |
| `Result.Error(Error error)` | Creates an error result from an Error object |
| `Result.From<T>(Result<T> result)` | Converts a generic result to non-generic |
| `bool IsOk()` | Returns true if the result represents success |
| `bool IsError()` | Returns true if the result represents an error |
| `Error ErrorValue` | Gets the error details (throws if result is success) |

### Result<T> (Generic)

| Method | Description |
|--------|-------------|
| `Result<T>.Ok(T value)` | Creates a successful result with a value |
| `Result<T>.Error(string message)` | Creates an error result with a message |
| `Result<T>.Error(string message, int code)` | Creates an error result with message and code |
| `Result<T>.Error(string message, HttpStatusCode code)` | Creates an error result with message and HTTP status code |
| `Result<T>.Error(Error error)` | Creates an error result from an Error object |
| `Result<T>.From<T1>(Result<T1> result)` | Converts between different generic result types (errors only) |
| `bool IsOk()` | Returns true if the result represents success |
| `bool IsError()` | Returns true if the result represents an error |
| `T Value` | Gets the success value (throws if result is error) |
| `Error ErrorValue` | Gets the error details (throws if result is success) |

### Error Structure

```csharp
public struct Error
{
    public string Message { get; set; }
    public int Code { get; set; }
}
```

## HTTP Response Behavior

When used as return types in ASP.NET Core controllers:

**Success Results:**
- `Result.Ok()` → HTTP 204 No Content
- `Result<T>.Ok(value)` → HTTP 200 OK with JSON serialized value

**Error Results:**
- Uses the error code as HTTP status code
- Returns JSON with error details:
```json
{
  "message": "Error description",
  "code": 404
}
```

## Best Practices

1. **Prefer Result over Exceptions**: Use Result types for expected error conditions
2. **Keep Error Messages User-Friendly**: Error messages may be exposed to end users
3. **Use HTTP Status Codes**: Leverage the HttpStatusCode enum for web APIs
4. **Check Results Before Accessing Values**: Always use `IsOk()` or `IsError()` before accessing `Value` or `ErrorValue`
5. **Convert When Needed**: Use `Result.From()` to convert between generic results

## Contributing

Contributions are welcome! Please feel free to submit issues, feature requests, or pull requests.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
