# Usage Patterns and Examples

## Registration

### Basic Registration
**Location**: `README.md:14`

```csharp
host.UseWolverine(opts =>
{
    opts.CodeGeneration.AddContinuationStrategy<CleanResultContinuationStrategy>();
});
```

This registers the continuation strategy with WolverineFx's code generation system.

## Handler Patterns

### Complete Handler Example
**Location**: `README.md:33`

```csharp
public record UpdateTodoCommand(Guid Id, string Title, string Description, bool IsCompleted);

public class UpdateTodoCommandHandler
{
    // Load method returns Result<T>
    public static async Task<Result<Todo>> LoadAsync(UpdateTodoCommand command, IQuerySession session)
    {
        var todo = await session.LoadAsync<Todo>(command.Id);
        if (todo is null)
            return Result.Error("Todos not found.", HttpStatusCode.NotFound);
        return Result.Ok(todo);
    }

    // Handle method receives extracted success value via DI
    public static async Task<Result<TodoUpdated>> Handle(UpdateTodoCommand command, Todo todo, IDocumentSession session)
    {
        command.Adapt(todo);

        session.Update(todo);
        await session.SaveChangesAsync();

        return Result.Ok(todo.Adapt<TodoUpdated>());
    }
}
```

### Key Pattern Elements

1. **Load Method**:
   - Named `Load` or `LoadAsync`
   - Returns `Result<T>` or `Result`
   - Handles data retrieval and validation
   - Returns errors for invalid states

2. **Handle Method**:
   - Receives the original command
   - **Receives extracted success value** (`Todo todo`) via dependency injection
   - Focuses on business logic only
   - No need for error checking from Load method

## Flow Control

### Success Flow
1. `LoadAsync` returns `Result.Ok(todo)`
2. Continuation strategy detects success
3. Extracts `todo` value and makes it available via DI
4. `Handle` method executes with extracted value
5. Returns final result

### Error Flow
1. `LoadAsync` returns `Result.Error(...)`
2. Continuation strategy detects error
3. **Skips `Handle` method execution entirely**
4. Returns error response to caller
5. No business logic is executed

## Supported Result Types

### Non-Generic Result
```csharp
public static async Task<Result> LoadAsync(SomeCommand command)
{
    // Validation logic
    if (someCondition)
        return Result.Error("Validation failed");
    
    return Result.Ok();
}

public static async Task<Result> Handle(SomeCommand command)
{
    // Business logic - only executed if Load succeeded
    return Result.Ok();
}
```

### Generic Result<T>
```csharp
public static async Task<Result<Entity>> LoadAsync(SomeCommand command)
{
    var entity = await GetEntityAsync(command.Id);
    if (entity == null)
        return Result.Error("Entity not found");
    
    return Result.Ok(entity);
}

public static async Task<Result<ResponseDto>> Handle(SomeCommand command, Entity entity)
{
    // 'entity' parameter is automatically injected from LoadAsync success value
    // Business logic here
    return Result.Ok(responseDto);
}
```

## Error Handling Patterns

### HTTP Status Code Errors
```csharp
return Result.Error("Resource not found", HttpStatusCode.NotFound);
return Result.Error("Validation failed", HttpStatusCode.BadRequest);
return Result.Error("Unauthorized access", HttpStatusCode.Unauthorized);
```

### Custom Error Types
```csharp
return Result.Error(new ValidationError("Field is required"));
return Result.Error(new BusinessRuleError("Cannot delete active entity"));
```

## Integration with Other Libraries

### Marten Integration
```csharp
public static async Task<Result<User>> LoadAsync(GetUserQuery query, IQuerySession session)
{
    var user = await session.LoadAsync<User>(query.UserId);
    return user != null ? Result.Ok(user) : Result.Error("User not found");
}
```

### Mapster Integration
```csharp
public static async Task<Result<UserDto>> Handle(GetUserQuery query, User user)
{
    var dto = user.Adapt<UserDto>();
    return Result.Ok(dto);
}
```

## Best Practices

### 1. Separation of Concerns
- **Load Methods**: Focus on data retrieval and validation
- **Handle Methods**: Focus on business logic and transformations

### 2. Error Consistency
- Use meaningful error messages
- Include appropriate HTTP status codes
- Consider custom error types for complex scenarios

### 3. Method Naming
- Use `Load`/`LoadAsync` for data loading methods
- Use `Handle`/`HandleAsync` for business logic methods

### 4. Dependency Injection
- Load method dependencies: Read-only services (IQuerySession)
- Handle method dependencies: Write services (IDocumentSession) + extracted values

### 5. Return Type Patterns
- Load methods: `Result<Entity>` for data retrieval
- Handle methods: `Result<ResponseDto>` for final responses
- Use non-generic `Result` for operations without return values

## Common Scenarios

### CRUD Operations
```csharp
// Create
public static async Task<Result> LoadAsync(CreateCommand cmd) => Result.Ok();
public static async Task<Result<CreatedDto>> Handle(CreateCommand cmd, IDocumentSession session) { ... }

// Read
public static async Task<Result<Entity>> LoadAsync(GetQuery query, IQuerySession session) { ... }
public static async Task<Result<EntityDto>> Handle(GetQuery query, Entity entity) { ... }

// Update
public static async Task<Result<Entity>> LoadAsync(UpdateCommand cmd, IQuerySession session) { ... }
public static async Task<Result<UpdatedDto>> Handle(UpdateCommand cmd, Entity entity, IDocumentSession session) { ... }

// Delete
public static async Task<Result<Entity>> LoadAsync(DeleteCommand cmd, IQuerySession session) { ... }
public static async Task<Result> Handle(DeleteCommand cmd, Entity entity, IDocumentSession session) { ... }
```