# CleanResult.Swashbuckle

![CleanResult](https://github.com/Gwynbleid85/CleanResult/blob/master/docs/readme-header.png?raw=true)

<div align="center">

[![NuGet](https://img.shields.io/nuget/v/CleanResult.Swashbuckle.svg)](https://www.nuget.org/packages/CleanResult.Swashbuckle/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

**Swagger/OpenAPI integration for CleanResult**

Clean OpenAPI documentation by automatically unwrapping Result types

[Main Documentation](../README.md) • [Features](#-features) • [Usage](#-usage) • [Examples](#-examples)

</div>

---

## 📦 Installation

```bash
dotnet add package CleanResult.Swashbuckle
```

**Requirements:**

- .NET 8.0 or later
- CleanResult 1.2.8+
- Swashbuckle.AspNetCore 6.0+

---

## ✨ Features

- 🎯 **Automatic Unwrapping** - Removes `Result<T>` wrapper from OpenAPI schemas
- 📄 **Clean Documentation** - Shows actual return types in Swagger UI
- 🔄 **HTTP Status Mapping** - Correctly maps Result.Ok() to 204, Result<T>.Ok() to 200
- 🧹 **Schema Cleanup** - Removes Result wrapper schemas from definitions
- ⚡ **Zero Configuration** - Works automatically after registration

---

## 🚀 Usage

### Registration

Add the CleanResult filters to your Swagger configuration:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen(options =>
{
    // ✅ Add CleanResult filters - automatically unwraps Result types
    options.AddCleanResultFilters();

    // Your other Swagger configuration...
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1"
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
```

### What It Does

The filters perform three key transformations:

1. **Operation Filter** - Unwraps Result types in endpoint responses
2. **Schema Filter** - Marks Result wrapper schemas for deletion
3. **Document Filter** - Removes marked schemas from final OpenAPI document

---

## 💡 Examples

### Before Integration

Without CleanResult.Swashbuckle, your Swagger documentation shows the Result wrapper:

```yaml
paths:
  /api/users/{id}:
    get:
      responses:
        '200':
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/ResultOfUser'

components:
  schemas:
    ResultOfUser:
      type: object
      properties:
        success: { type: boolean }
        successValue: { $ref: '#/components/schemas/User' }
        internalErrorValue: { $ref: '#/components/schemas/Error' }
```

### After Integration

With CleanResult.Swashbuckle, documentation is clean and shows actual types:

```yaml
paths:
  /api/users/{id}:
    get:
      responses:
        '200':
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/User'

components:
  schemas:
    User:
      type: object
      properties:
        id: { type: integer }
        name: { type: string }
        email: { type: string }
```

### Controller Examples

```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    // ✅ Swagger shows: 200 OK → User schema
    [HttpGet("{id}")]
    public Result<User> GetUser(int id)
    {
        return Result.Ok(new User { Id = id, Name = "John" });
    }

    // ✅ Swagger shows: 204 No Content
    [HttpPost]
    public Result CreateUser([FromBody] CreateUserDto dto)
    {
        return Result.Ok();
    }

    // ✅ Swagger shows: 200 OK → List<User> schema
    [HttpGet]
    public Result<List<User>> GetUsers()
    {
        return Result.Ok(new List<User>());
    }

    // ✅ Swagger shows: 200 OK → PagedResult<User> schema
    [HttpGet("paged")]
    public Result<PagedResult<User>> GetPagedUsers([FromQuery] int page = 1)
    {
        return Result.Ok(new PagedResult<User>());
    }
}
```

### Response Status Mapping

The filter correctly maps Result types to HTTP status codes:

| Method Return Type | Swagger Status | Content-Type             | Schema |
|--------------------|----------------|--------------------------|--------|
| `Result`           | 204 No Content | -                        | None   |
| `Result<User>`     | 200 OK         | application/json         | User   |
| `Result<string>`   | 200 OK         | text/plain               | string |
| `Result<byte[]>`   | 200 OK         | application/octet-stream | binary |
| `Task<Result>`     | 204 No Content | -                        | None   |
| `Task<Result<T>>`  | 200 OK         | varies                   | T      |

---

## 🔧 How It Works

### Three-Phase Filtering

**1. Operation Filter (`CleanResultReturnTypeFilter`)**

```csharp
// Examines controller method return types
// For Result: Changes 200 → 204 (No Content)
// For Result<T>: Keeps 200 but replaces schema with T's schema
// Unwraps from Task<Result> or Task<Result<T>>
```

**2. Schema Filter (`CleanResultSchemaFilter`)**

```csharp
// Marks Result type schemas with special title: "SchemaToDelete"
// Prevents Result wrapper types from appearing in schema definitions
```

**3. Document Filter (`CleanResultReturnDocumentFilter`)**

```csharp
// Final cleanup phase
// Removes all schemas marked "SchemaToDelete"
// Ensures clean OpenAPI document
```

### Architecture

```
Swagger Generation
        ↓
Operation Filter
  (unwrap types)
        ↓
Schema Filter
  (mark wrappers)
        ↓
Document Filter
  (remove marked)
        ↓
Clean OpenAPI Doc
```

---

## 🎯 Best Practices

### ✅ Do's

```csharp
// ✅ Use Result types directly
[HttpGet("{id}")]
public Result<User> GetUser(int id)
    => _service.GetById(id);

// ✅ Works with async
[HttpGet("{id}")]
public async Task<Result<User>> GetUserAsync(int id)
    => await _service.GetByIdAsync(id);

// ✅ Complex types are unwrapped correctly
[HttpGet]
public Result<PagedList<UserDto>> GetPagedUsers([FromQuery] PagingParams params)
    => _service.GetPaged(params);
```

### ❌ Don'ts

```csharp
// ❌ Don't wrap Result in IActionResult
[HttpGet("{id}")]
public IActionResult GetUser(int id)
{
    var result = _service.GetById(id);
    return Ok(result);  // Bad: Swagger won't unwrap
}

// ❌ Don't return Result as object
[HttpGet("{id}")]
public object GetUser(int id)
    => _service.GetById(id);  // Bad: type information lost
```

---

## 📖 Advanced Configuration

### Custom Swagger Options

```csharp
builder.Services.AddSwaggerGen(options =>
{
    // ✅ Add CleanResult filters first
    options.AddCleanResultFilters();

    // Then add your custom configuration
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1",
        Description = "API with CleanResult integration"
    });

    // Add XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    // Add authorization
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
});
```

### Response Examples

```csharp
/// <summary>
/// Gets a user by ID
/// </summary>
/// <param name="id">User ID</param>
/// <returns>The user if found</returns>
/// <response code="200">Returns the user</response>
/// <response code="404">User not found</response>
[HttpGet("{id}")]
[ProducesResponseType(typeof(User), 200)]
[ProducesResponseType(typeof(Error), 404)]
public Result<User> GetUser(int id)
{
    var user = _repository.FindById(id);
    if (user == null)
        return Result<User>.Error("User not found", 404);

    return Result.Ok(user);
}
```

---

## 🔗 Related Packages

- **[CleanResult](../README.md)** - Core Result implementation
- **[CleanResult.FluentValidation](../CleanResult.FluentValidation/README.md)** - FluentValidation integration
- **[CleanResult.WolverineFx](../CleanResult.WolverineFx/README.md)** - WolverineFx messaging integration
- **[CleanResult.AspNet](../CleanResult.AspNet/README.md)** - IActionResult adapter

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](../LICENSE) file for details.

---

<div align="center">

**[⬆ Back to Top](#cleanresultswashbuckle)** • **[Main Documentation](../README.md)**

</div>

<div align="center">
Gwynbleid85 © 2025
</div>