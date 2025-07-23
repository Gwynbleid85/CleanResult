# WolverineTests

This project demonstrates and tests the CleanResult.WolverineFx continuation strategy with various scenarios.

## Test Scenarios

### 1. Simple Result Tests (`SimpleResultTests.cs`)
- Tests handlers with `LoadAsync` returning `Result` (non-generic)
- Demonstrates error handling in the Load phase
- Shows continuation to Handle method when Load succeeds

### 2. Generic Result Tests (`GenericResultTests.cs`)
- Tests handlers with `LoadAsync` returning `Result<User>`
- Shows automatic extraction of success values for Handle method parameters
- Tests error propagation when Load fails

### 3. Tuple Result Tests (`TupleResultTests.cs`)
- Tests handlers with `LoadAsync` returning `Result<(User, Product)>`
- Demonstrates tuple value extraction by the continuation strategy
- Shows handling of complex return types

## Handler Patterns

All handlers follow the Wolverine pattern:

```csharp
public class MyHandler
{
    // Load method - returns Result or Result<T>
    public static async Task<Result<Entity>> LoadAsync(Command cmd)
    {
        // Validation/loading logic
        return entity != null ? Result.Ok(entity) : Result.Error("Not found");
    }

    // Handle method - receives extracted success value(s)
    public static async Task<Result<ResponseDto>> Handle(Command cmd, Entity entity)
    {
        // Business logic with extracted entity
        return Result.Ok(new ResponseDto());
    }
}
```

## Running Tests

```bash
# Run all tests
dotnet test WolverineTests/WolverineTests.csproj

# Run specific test class
dotnet test WolverineTests/WolverineTests.csproj --filter "ClassName~SimpleResultTests"
```

## Key Features Tested

1. **Error Short-Circuiting**: When Load methods return errors, Handle methods are not executed
2. **Value Extraction**: Success values from `Result<T>` are automatically extracted as Handle method parameters
3. **Tuple Support**: Tuple values are properly extracted and made available to Handle methods
4. **Error Propagation**: Errors maintain their status codes and messages through the pipeline