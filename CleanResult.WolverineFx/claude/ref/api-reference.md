# API Reference

## CleanResultContinuationStrategy

### Class Overview
```csharp
namespace CleanResult.WolverineFx;

public class CleanResultContinuationStrategy : IContinuationStrategy
```

**Purpose**: Provides automatic handling of CleanResult types in WolverineFx message handlers.

**Remarks**: Designed primarily for use with `IMessageBus.InvokeAsync<T>` methods. Other invocation patterns (Publish, Send) have not been tested.

### Public Methods

#### TryFindContinuationHandler
```csharp
public bool TryFindContinuationHandler(IChain chain, MethodCall call, out Frame? frame)
```

**Parameters**:
- `chain` (`IChain`): The method chain being processed
- `call` (`MethodCall`): The method call to analyze  
- `frame` (`Frame?`): Output parameter containing the continuation frame if found

**Returns**: `bool` - `true` if continuation handling is needed, `false` otherwise

**Behavior**:
1. Checks if method returns `Result` type → creates `MaybeEndHandlerWithCleanResultFrame`
2. Checks if method returns `Result<T>` type → creates `MaybeEndHandlerWithGenericCleanResultFrame`
3. Returns `false` if no Result types are detected

**Location**: `CleanResultContinuationStrategy.cs:27`

### Private Methods

#### GetFriendlyTypeName
```csharp
private static string GetFriendlyTypeName(Type type)
```

**Purpose**: Converts Type objects to code-generation friendly string representations.

**Parameters**:
- `type` (`Type`): The type to convert

**Returns**: `string` - Formatted type name suitable for code generation

**Examples**:
- `Result` → `"CleanResult.Result"`
- `Result<User>` → `"CleanResult.Result<User>"`
- `Result<List<string>>` → `"CleanResult.Result<System.Collections.Generic.List<string>>"`

**Location**: `CleanResultContinuationStrategy.cs:55`

#### GetHandlerReturnType
```csharp
private Type? GetHandlerReturnType(MethodCall call)
```

**Purpose**: Extracts the return type from Handle/HandleAsync methods.

**Parameters**:
- `call` (`MethodCall`): The method call context

**Returns**: `Type?` - The handler's return type, unwrapped from Task if necessary

**Behavior**:
- Searches for methods ending with "Handle" or "HandleAsync"
- Unwraps `Task<T>` to get the inner type `T`
- Returns `null` if no handler method is found

**Location**: `CleanResultContinuationStrategy.cs:73`

## Frame Classes

### MaybeEndHandlerWithCleanResultFrame

```csharp
private class MaybeEndHandlerWithCleanResultFrame : AsyncFrame
```

**Purpose**: Handles non-generic `Result` types in the continuation pipeline.

#### Constructor
```csharp
public MaybeEndHandlerWithCleanResultFrame(Variable result, Type? handlerReturnType)
```

**Parameters**:
- `result` (`Variable`): The Result variable to check
- `handlerReturnType` (`Type?`): The expected return type of the handler

#### Generated Code
```csharp
// CleanResult continuation check for Result types
if ({result}.IsError())
{
    await context.EnqueueCascadingAsync({HandlerReturnType}.Error({result}.ErrorValue)).ConfigureAwait(false);
    return;
}
```

**Location**: `CleanResultContinuationStrategy.cs:86`

### MaybeEndHandlerWithGenericCleanResultFrame

```csharp
private class MaybeEndHandlerWithGenericCleanResultFrame : AsyncFrame
```

**Purpose**: Handles generic `Result<T>` types with automatic value extraction.

#### Constructor
```csharp
public MaybeEndHandlerWithGenericCleanResultFrame(Variable result, Type? handlerReturnType)
```

**Parameters**:
- `result` (`Variable`): The Result<T> variable to check
- `handlerReturnType` (`Type?`): The expected return type of the handler

#### Generated Code
```csharp
// CleanResult continuation check for Result<T> types
if ({result}.IsError())
{
    await context.EnqueueCascadingAsync({HandlerReturnType}.Error({result}.ErrorValue)).ConfigureAwait(false);
    return;
}

// Extracting the success value from Result<T>
var {result}SuccessValue = {result}.Value;
```

#### Variable Creation
Automatically creates a new variable:
- **Name**: `{originalVariableName}SuccessValue`
- **Type**: The generic type argument from `Result<T>`
- **Availability**: Injected into subsequent handler methods

**Location**: `CleanResultContinuationStrategy.cs:122`

## Registration API

### WolverineFx Configuration
```csharp
host.UseWolverine(opts =>
{
    opts.CodeGeneration.AddContinuationStrategy<CleanResultContinuationStrategy>();
});
```

**Method**: `AddContinuationStrategy<T>()`
**Namespace**: `Wolverine.Configuration`
**Purpose**: Registers the continuation strategy with WolverineFx's code generation system

## Handler Conventions

### Method Naming
- **Load Methods**: Must be named `Load` or `LoadAsync`
- **Handle Methods**: Must be named `Handle` or `HandleAsync`

### Return Types
- **Supported Load Returns**: `Result`, `Result<T>`, `Task<Result>`, `Task<Result<T>>`
- **Handle Parameters**: Original command + extracted success values (for `Result<T>`)

### Parameter Injection
For `Result<T>` returns from Load methods:
- Success value is extracted as `{variableName}SuccessValue`
- Becomes available for dependency injection in Handle methods
- Maintains original type information

## Error Handling API

### Error Detection
Uses `Result.IsError()` method to detect error conditions.

### Error Propagation
```csharp
await context.EnqueueCascadingAsync(errorResult).ConfigureAwait(false);
```

**Method**: `EnqueueCascadingAsync()`
**Context**: WolverineFx message context
**Purpose**: Propagates error responses back to the caller

### Type Conversion
When handler return type differs from Load return type:
```csharp
{HandlerReturnType}.Error({loadResult}.ErrorValue)
```

## Dependencies

### Required Namespaces
```csharp
using JasperFx.CodeGeneration;
using JasperFx.CodeGeneration.Frames;
using JasperFx.CodeGeneration.Model;
using Wolverine;
using Wolverine.Configuration;
using Wolverine.Middleware;
```

### Framework Dependencies
- **WolverineFx (4.5.2+)**: Core messaging framework
- **CleanResult (1.2.2+)**: Result pattern library
- **JasperFx.CodeGeneration**: Code generation infrastructure

## Limitations and Constraints

### Method Constraints
- Must use Load/LoadAsync naming convention
- Handle methods must accept extracted values as parameters
- Designed for InvokeAsync usage patterns

### Type Constraints
- Only supports `Result` and `Result<T>` types
- Generic constraints follow CleanResult library limitations
- Return types must be compatible with WolverineFx expectations

### Framework Constraints
- Requires WolverineFx code generation to be enabled
- Must be registered before application startup
- Cannot be dynamically enabled/disabled at runtime