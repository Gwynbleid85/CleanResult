# CleanResult.AspNet

![CleanResult](https://raw.githubusercontent.com/Gwynbleid85/CleanResult/refs/heads/master/docs/readme-header.png)

<div align="center">

[![NuGet](https://img.shields.io/nuget/v/CleanResult.AspNet.svg)](https://www.nuget.org/packages/CleanResult.AspNet/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-8.0%20%7C%209.0%20%7C%2010.0-purple.svg)](https://dotnet.microsoft.com/download)

[![Build](https://img.shields.io/github/actions/workflow/status/Gwynbleid85/CleanResult/publish.yml?logo=github&label=build)](https://github.com/Gwynbleid85/CleanResult/actions/workflows/publish.yml)
[![Testing](https://img.shields.io/github/actions/workflow/status/Gwynbleid85/CleanResult/publish.yml?logo=github&label=testing)](https://github.com/Gwynbleid85/CleanResult/actions/workflows/publish.yml)

**IActionResult adapter for CleanResult**

Explicit `IActionResult` conversion for traditional ASP.NET Core controllers

[Main Documentation](../README.md) • [Features](#-features) • [Usage](#-usage) • [When to Use](#-when-to-use-this-package)

</div>

---

## 📦 Installation

```bash
dotnet add package CleanResult.AspNet
```

**Requirements:**

- .NET 8.0, 9.0, or 10.0
- ASP.NET Core 8.0+

---

## ✨ Features

- 🔄 **IActionResult Adapter** - Explicit conversion from Result to IActionResult
- 🎯 **Controller Compatibility** - Works with traditional controller patterns
- ⚡ **Zero Overhead** - Thin wrapper around existing IResult implementation
- 📄 **Clean Integration** - Maintains all CleanResult features and behaviors

---

## 🚀 Usage

### Basic Conversion

```csharp
using CleanResult.AspNet;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    // ✅ Convert Result to IActionResult
    [HttpGet("{id}")]
    public IActionResult GetUser(int id)
    {
        var result = _userService.GetById(id);
        return result.ToIActionResult();
    }

    // ✅ Convert Result<T> to IActionResult
    [HttpPost]
    public IActionResult CreateUser([FromBody] CreateUserDto dto)
    {
        var result = _userService.Create(dto);
        return result.ToIActionResult();
    }
}
```

### Direct Return Pattern

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    [HttpGet("{id}")]
    public IActionResult GetProduct(int id)
        => _productService.GetById(id).ToIActionResult();

    [HttpPut("{id}")]
    public IActionResult UpdateProduct(int id, [FromBody] UpdateProductDto dto)
        => _productService.Update(id, dto).ToIActionResult();

    [HttpDelete("{id}")]
    public IActionResult DeleteProduct(int id)
        => _productService.Delete(id).ToIActionResult();
}
```

---

## 🤔 When to Use This Package

### ✅ Use CleanResult.AspNet When

- You're working with **existing controllers** that return `IActionResult`
- You need **explicit conversion** for compatibility
- Your team prefers **traditional controller patterns**
- You're **gradually migrating** to CleanResult

```csharp
// ✅ Good use case: existing IActionResult pattern
public IActionResult GetUser(int id)
{
    var result = _service.GetUser(id);
    return result.ToIActionResult();
}
```

### ❌ Don't Use When

- You're using **minimal APIs** → Use core CleanResult directly (implements `IResult`)
- You can return **Result types directly** → Core CleanResult already implements `IResult`

```csharp
// ❌ Unnecessary - CleanResult already implements IResult
[HttpGet("{id}")]
public Result<User> GetUser(int id)  // Direct return works!
    => _service.GetUser(id);

// ❌ Unnecessary conversion
public Result<User> GetUser(int id)
{
    var result = _service.GetUser(id);
    return result.ToIActionResult();  // Don't do this!
}
```

---

## 💡 Examples

### Service Layer Integration

```csharp
public interface IUserService
{
    Result<User> GetById(int id);
    Result<User> Create(CreateUserDto dto);
    Result Update(int id, UpdateUserDto dto);
    Result Delete(int id);
}

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{id}")]
    public IActionResult GetUser(int id){
        return _userService.GetById(id).ToIActionResult();
    }

    [HttpPost]
    public IActionResult CreateUser([FromBody] CreateUserDto dto){
        return _userService.Create(dto).ToIActionResult();
    }

    [HttpPut("{id}")]
    public IActionResult UpdateUser(int id, [FromBody] UpdateUserDto dto){
        return _userService.Update(id, dto).ToIActionResult();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteUser(int id){ 
        return _userService.Delete(id).ToIActionResult();
    }
}
```

### Async Patterns

```csharp
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    // ✅ Async with IActionResult
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(int id)
    {
        var result = await _orderService.GetByIdAsync(id);
        return result.ToIActionResult();
    }

    // ✅ Direct async pattern
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        => (await _orderService.CreateAsync(dto)).ToIActionResult();
}
```

### Error Handling

```csharp
[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    [HttpPost("process")]
    public IActionResult ProcessPayment([FromBody] PaymentDto dto)
    {
        // Service returns Result<PaymentConfirmation>
        var result = _paymentService.Process(dto);

        // Automatically handles:
        // - Success: HTTP 200 with PaymentConfirmation
        // - Error: HTTP status from error with Problem Details
        return result.ToIActionResult();
    }
}
```

---

## 🔧 How It Works

### Implementation

The package provides thin wrapper classes that delegate to the underlying `IResult` implementation:

```csharp
// For non-generic Result
public static IActionResult ToIActionResult(this Result result)
    => new AspActionResult(result);

// For generic Result<T>
public static IActionResult ToIActionResult<T>(this Result<T> result)
    => new AspActionResult<T>(result);
```

The wrapper classes implement `IActionResult` by calling the Result's `ExecuteAsync` method:

```csharp
internal class AspActionResult<T> : IActionResult
{
    private readonly Result<T> _result;

    public Task ExecuteResultAsync(ActionContext context)
        => _result.ExecuteAsync(context.HttpContext);
}
```

### Response Behavior

| Result Type                 | HTTP Status    | Content-Type             | Body                       |
|-----------------------------|----------------|--------------------------|----------------------------|
| `Result.Ok()`               | 204 No Content | application/json         | Empty                      |
| `Result<string>.Ok("text")` | 200 OK         | text/plain               | Raw string                 |
| `Result<User>.Ok(user)`     | 200 OK         | application/json         | JSON object                |
| `Result<byte[]>.Ok(data)`   | 200 OK         | application/octet-stream | Binary data                |
| `Result<Stream>.Ok(stream, contentType, fileDownloadName)` | 200 OK | configured content type | Streamed file              |
| `Result.Error("msg", 404)`  | 404 Not Found  | application/json         | Problem Details (RFC 9457) |

---

## 🎯 Best Practices

### ✅ Do's

```csharp
// ✅ Use for existing IActionResult controllers
[HttpGet("{id}")]
public IActionResult GetUser(int id)
    => _service.GetUser(id).ToIActionResult();

// ✅ Use when you need IActionResult explicitly
public IActionResult PerformAction()
{
    var result = _service.DoSomething();
    if (someCondition)
    {
        return Redirect("/somewhere");
    }
    return result.ToIActionResult();
}

// ✅ Consistent with service layer returning Results
public IActionResult ProcessRequest([FromBody] RequestDto dto)
{
    var validationResult = _validator.Validate(dto);
    if (validationResult.IsError())
        return validationResult.ToIActionResult();

    return _service.Process(dto).ToIActionResult();
}
```

### ❌ Don'ts

```csharp
// ❌ Don't use when you can return Result directly
[HttpGet("{id}")]
public Result<User> GetUser(int id)  // Better: return Result directly
    => _service.GetUser(id).ToIActionResult();

// ❌ Don't double-wrap in IActionResult
[HttpGet("{id}")]
public IActionResult GetUser(int id)
{
    var result = _service.GetUser(id).ToIActionResult();
    return Ok(result);  // Bad: unnecessary nesting
}

// ❌ Don't use for minimal APIs
app.MapGet("/users/{id}", (int id) =>
{
    var result = _service.GetUser(id);
    return result.ToIActionResult();  // Unnecessary - use Result directly
});
```

---

## 🔗 Related Packages

- **[CleanResult](../README.md)** - Core Result implementation
- **[CleanResult.FluentValidation](../CleanResult.FluentValidation/README.md)** - FluentValidation integration
- **[CleanResult.WolverineFx](../CleanResult.WolverineFx/README.md)** - WolverineFx messaging integration
- **[CleanResult.Swashbuckle](../CleanResult.Swashbuckle/README.md)** - Swagger/OpenAPI integration

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](../LICENSE) file for details.

---

<div align="center">

**[⬆ Back to Top](#cleanresultaspnet)** • **[Main Documentation](../README.md)**

</div>

<div align="center">
Gwynbleid85 © 2025
</div>
