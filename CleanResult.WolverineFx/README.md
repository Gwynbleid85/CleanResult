# CleanResult.WolverineFx

![CleanResult](https://raw.githubusercontent.com/Gwynbleid85/CleanResult/refs/heads/master/docs/readme-header.png)

<div align="center">

[![NuGet](https://img.shields.io/nuget/v/CleanResult.WolverineFx.svg)](https://www.nuget.org/packages/CleanResult.WolverineFx/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

**WolverineFx messaging integration for CleanResult**

Automatic Result handling in WolverineFx message handlers with compile-time code generation

[Main Documentation](../README.md) • [Features](#-features) • [Usage](#-usage) • [How It Works](#-how-it-works)

</div>

---

## 📦 Installation

```bash
dotnet add package CleanResult.WolverineFx
```

**Requirements:**

- .NET 8.0 or 9.0
- WolverineFx 3.0+

> **Note:** .NET 10.0 support is pending WolverineFx compatibility updates.

---

## ✨ Features

- 🚀 **Automatic Error Handling** - Short-circuits on errors without boilerplate
- 🎯 **Value Extraction** - Extracts success values from `Result<T>` automatically
- ⚡ **Zero Runtime Overhead** - Uses compile-time code generation (no reflection)
- 🔄 **Type Conversion** - Automatically converts errors between Result types
- 📦 **Tuple Support** - Handles tuple return values from Result<(T1, T2)>
- 🔍 **Query Helpers** - Special helpers for query patterns (null checks)

---

## 🚀 Usage

### Registration

Register the continuation strategy in your WolverineFx configuration:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Host.UseWolverine(opts =>
{
    // ✅ Add CleanResult continuation strategy
    opts.CodeGeneration.AddContinuationStrategy<CleanResultContinuationStrategy>();
});

var app = builder.Build();
app.Run();
```

### Basic Handler Pattern

WolverineFx handlers can use `Load`/`LoadAsync` methods that return `Result<T>`. The framework automatically:

1. Calls Load/LoadAsync
2. Checks for errors
3. Extracts the success value
4. Passes it to Handle method

```csharp
public class CreateUserHandler
{
    // ✅ Load returns Result<T> - validated before Handle is called
    public static async Task<Result<User>> LoadAsync(CreateUserCommand command)
    {
        // Validation logic
        if (string.IsNullOrEmpty(command.Email))
            return Result<User>.Error("Email is required", 400);

        var existing = await _repository.FindByEmailAsync(command.Email);
        if (existing != null)
            return Result<User>.Error("Email already exists", 409);

        // Return new user to be created
        return Result.Ok(new User { Email = command.Email, Name = command.Name });
    }

    // ✅ Handle receives the extracted User only if Load succeeded
    public static async Task<Result<UserCreatedEvent>> Handle(
        CreateUserCommand command,
        User user)  // This is the extracted value from LoadAsync!
    {
        await _repository.SaveAsync(user);
        return Result.Ok(new UserCreatedEvent(user.Id, user.Email));
    }
}
```

### Generated Code

The framework generates code similar to this:

```csharp
var loadResult = await CreateUserHandler.LoadAsync(command);

// Automatic error check
if (loadResult.IsError())
{
    await context.EnqueueCascadingAsync(
        Result<UserCreatedEvent>.Error(loadResult.ErrorValue)
    );
    return;
}

// Extract value and pass to Handle
var user = loadResult.Value;
var handleResult = await CreateUserHandler.Handle(command, user);
await context.EnqueueCascadingAsync(handleResult);
```

### Tuple Support

```csharp
public class OrderHandler
{
    // ✅ Return multiple values as tuple
    public static async Task<Result<(User, Product)>> LoadAsync(CreateOrderCommand command)
    {
        var user = await _userRepo.FindAsync(command.UserId);
        if (user == null)
            return Result.Error<(User, Product)>("User not found", 404);

        var product = await _productRepo.FindAsync(command.ProductId);
        if (product == null)
            return Result.Error<(User, Product)>("Product not found", 404);

        return Result.Ok((user, product));
    }

    // ✅ Handle receives both values automatically
    public static async Task<Result<Order>> Handle(
        CreateOrderCommand command,
        User user,      // Extracted from tuple
        Product product)  // Extracted from tuple
    {
        if (product.Stock < command.Quantity)
            return Result<Order>.Error("Insufficient stock", 409);

        var order = new Order
        {
            UserId = user.Id,
            ProductId = product.Id,
            Quantity = command.Quantity
        };

        await _orderRepo.SaveAsync(order);
        return Result.Ok(order);
    }
}
```

### Query Helpers

Special helpers for query patterns where null indicates "not found":

```csharp
public class GetUserHandler
{
    public static async Task<Result<User>> LoadAsync(GetUserQuery query)
    {
        var user = await _repository.FindByIdAsync(query.UserId);

        // ✅ Check if error OR null
        if (user.IsQueryError())
            return Result<User>.Error("User not found", 404);

        return Result.Ok(user);
    }

    // Alternative: automatically convert null to 404
    public static async Task<Result<User>> LoadAsync(GetUserQuery query)
    {
        var user = await _repository.FindByIdAsync(query.UserId);

        // ✅ Converts null to 404 error automatically
        return user.ToQueryError("User not found");
    }
}
```

---

## 🔧 How It Works

### Continuation Strategy

CleanResult.WolverineFx implements a custom `IContinuationStrategy` that hooks into WolverineFx's code generation
pipeline:

1. **Detection Phase** - Scans for Load/LoadAsync methods returning `Result` or `Result<T>`
2. **Frame Creation** - Creates continuation frames that inject error-checking code
3. **Code Generation** - Generates C# code that:
    - Calls the Load method
    - Checks `IsError()`
    - Short-circuits on error with proper type conversion
    - Extracts success values
    - Passes values to Handle method

### Architecture

```
Message arrives → LoadAsync called
                       ↓
                 Returns Result<T>
                       ↓
                 [Generated Code]
                IsError() check
                       ↓
         ┌─────────────┴─────────────┐
         ↓ Error                    ↓ Success
    Convert error type         Extract value
    Return immediately         Pass to Handle
                                     ↓
                              Handle executes
                                     ↓
                              Return Result
```

---

## 💡 Examples

### Complete Handler

```csharp
public record UpdateProductCommand(Guid ProductId, string Name, decimal Price, Guid CategoryId);

public class UpdateProductHandler
{
    private readonly IProductRepository _products;
    private readonly ICategoryRepository _categories;

    public async Task<Result<(Product, Category)>> LoadAsync(
        UpdateProductCommand command,
        IQuerySession session)
    {
        // Load and validate product
        var product = await session.LoadAsync<Product>(command.ProductId);
        if (product == null)
            return Result.Error<(Product, Category)>("Product not found", 404);

        // Load and validate category
        var category = await session.LoadAsync<Category>(command.CategoryId);
        if (category == null)
            return Result.Error<(Product, Category)>("Category not found", 404);

        return Result.Ok((product, category));
    }

    public async Task<Result<ProductDto>> Handle(
        UpdateProductCommand command,
        Product product,
        Category category,
        IDocumentSession session)
    {
        // Update product
        product.Name = command.Name;
        product.Price = command.Price;
        product.CategoryId = category.Id;

        session.Update(product);
        await session.SaveChangesAsync();

        return Result.Ok(new ProductDto(product));
    }
}
```

### Error Handling Patterns

```csharp
public class PlaceOrderHandler
{
    public static async Task<Result<OrderContext>> LoadAsync(
        PlaceOrderCommand command,
        IQuerySession session)
    {
        // Multiple validation steps
        var user = await session.LoadAsync<User>(command.UserId);
        if (user == null)
            return Result.Error("Invalid user", 400);

        var product = await session.LoadAsync<Product>(command.ProductId);
        if (product == null)
            return Result.Error("Invalid product", 400);

        if (product.Stock < command.Quantity)
            return Result.Error("Insufficient stock", 409);

        // Return context with validated data
        return Result.Ok(new OrderContext(user, product, command.Quantity));
    }

    public static async Task<Result<OrderConfirmation>> Handle(
        PlaceOrderCommand command,
        OrderContext context, // This is the extracted value from LoadAsync
        IDocumentSession session)
    {
        // Business logic with validated data
        var order = new Order
        {
            UserId = context.User.Id,
            ProductId = context.Product.Id,
            Quantity = context.Quantity
        };

        session.Store(order);

        // Update stock
        context.Product.Stock -= context.Quantity;
        session.Update(context.Product);

        await session.SaveChangesAsync();

        return Result.Ok(new OrderConfirmation(order.Id));
    }
}
```

---

## 🎯 Best Practices

### ✅ Do's

```csharp
// ✅ Use Load for validation and data loading
public static async Task<Result<Data>> LoadAsync(Command cmd, IQuerySession session)
{
    var data = await session.LoadAsync<Data>(cmd.Id);
    if (data == null)
        return Result<Data>.Error("Not found", 404);
    return Result.Ok(data);
}

// ✅ Use Handle for business logic with validated data
public static async Task<Result<Response>> Handle(
    Command cmd,
    Data data,
    IDocumentSession session)
{
    // Business logic here
    session.Update(data);
    await session.SaveChangesAsync();
    return Result.Ok(new Response());
}

// ✅ Return tuples for multiple dependencies
public static async Task<Result<(User, Order)>> LoadAsync(
    Command cmd,
    IQuerySession session)
{
    var user = await session.LoadAsync<User>(cmd.UserId);
    var order = await session.LoadAsync<Order>(cmd.OrderId);
    return Result.Ok((user, order));
}
```

### ❌ Don'ts

```csharp
// ❌ Don't put business logic in Load
public static async Task<Result<User>> LoadAsync(Command cmd, IDocumentSession session)
{
    var user = await session.LoadAsync<User>(cmd.Id);
    session.Update(user);  // Bad: side effects in Load
    return Result.Ok(user);
}

// ❌ Don't skip validation in Load
public static async Task<Result<User>> LoadAsync(Command cmd, IQuerySession session)
{
    var user = await session.LoadAsync<User>(cmd.Id);
    return Result.Ok(user);  // Bad: didn't check if null
}

// ❌ Don't access Value without checking in custom code
var result = await LoadAsync(cmd, session);
var value = result.Value;  // Bad: might throw
// Let the framework handle value extraction
```

---

## 🔗 Related Packages

- **[CleanResult](../README.md)** - Core Result implementation
- **[CleanResult.FluentValidation](../CleanResult.FluentValidation/README.md)** - FluentValidation integration
- **[CleanResult.Swashbuckle](../CleanResult.Swashbuckle/README.md)** - Swagger/OpenAPI integration
- **[CleanResult.AspNet](../CleanResult.AspNet/README.md)** - IActionResult adapter

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](../LICENSE) file for details.

---

<div align="center">

**[⬆ Back to Top](#cleanresultwolverinefx)** • **[Main Documentation](../README.md)**

</div>

<div align="center">
Gwynbleid85 © 2025
</div>
