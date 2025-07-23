# Core Implementation Reference

## CleanResultContinuationStrategy Class

**Location**: `CleanResultContinuationStrategy.cs:25`

The main implementation class that integrates CleanResult types with WolverineFx's message handling pipeline.

### Class Declaration
```csharp
public class CleanResultContinuationStrategy : IContinuationStrategy
```

### Purpose
- Implements `IContinuationStrategy` interface from WolverineFx
- Provides custom handling for `Result` and `Result<T>` types in message handlers
- Automatically manages error conditions and success value extraction

### Key Methods

#### TryFindContinuationHandler
**Location**: `CleanResultContinuationStrategy.cs:27`

```csharp
public bool TryFindContinuationHandler(IChain chain, MethodCall call, out Frame? frame)
```

**Purpose**: Detects if a method call returns Result types and creates appropriate continuation frames.

**Logic**:
1. Checks for non-generic `Result` return types → creates `MaybeEndHandlerWithCleanResultFrame`
2. Checks for generic `Result<T>` return types → creates `MaybeEndHandlerWithGenericCleanResultFrame`
3. Returns `false` if no Result types are found

#### GetFriendlyTypeName
**Location**: `CleanResultContinuationStrategy.cs:55`

```csharp
private static string GetFriendlyTypeName(Type type)
```

**Purpose**: Converts generic types to code-generation friendly string representations.

**Handles**:
- Non-generic types: Returns `FullName` or `Name`
- Generic types: Formats as `TypeName<GenericArg1,GenericArg2>`

#### GetHandlerReturnType
**Location**: `CleanResultContinuationStrategy.cs:73`

```csharp
private Type? GetHandlerReturnType(MethodCall call)
```

**Purpose**: Extracts the actual return type from Handle/HandleAsync methods.

**Logic**:
- Finds methods ending with "Handle" or "HandleAsync"
- Unwraps `Task<T>` types to get the inner type
- Returns the handler's return type for proper type conversion

## Frame Implementations

### MaybeEndHandlerWithCleanResultFrame
**Location**: `CleanResultContinuationStrategy.cs:86`

Handles non-generic `Result` types.

#### Generated Code Pattern
```csharp
// CleanResult continuation check for Result types
if (result.IsError())
{
    await context.EnqueueCascadingAsync(ResultType.Error(result.ErrorValue)).ConfigureAwait(false);
    return;
}
```

#### Key Features
- Checks `IsError()` condition
- Enqueues error responses using `EnqueueCascadingAsync`
- Performs type conversion if handler return type differs
- Terminates handler execution on error

### MaybeEndHandlerWithGenericCleanResultFrame
**Location**: `CleanResultContinuationStrategy.cs:122`

Handles generic `Result<T>` types with value extraction.

#### Generated Code Pattern
```csharp
// CleanResult continuation check for Result<T> types
if (result.IsError())
{
    await context.EnqueueCascadingAsync(ResultType.Error(result.ErrorValue)).ConfigureAwait(false);
    return;
}

// Extracting the success value from Result<T>
var resultSuccessValue = result.Value;
```

#### Key Features
- Same error handling as non-generic version
- **Value Extraction**: Creates new variable with success value
- Variable naming pattern: `{originalName}SuccessValue`
- Makes extracted value available for dependency injection

## Code Generation Details

### Variable Management
- **Uses**: Tracks variables consumed by the frame
- **Creates**: Registers new variables for success values
- **Type Safety**: Maintains proper type information for extracted values

### Async Handling
- Uses `ConfigureAwait(false)` for library code best practices
- Proper async/await patterns throughout
- Compatible with WolverineFx's async pipeline

### Error Propagation
- Errors are propagated using `context.EnqueueCascadingAsync`
- Maintains error context and HTTP status codes
- Supports custom error types and messages

## Integration Points

### WolverineFx Integration
- Implements `IContinuationStrategy` interface
- Integrates with WolverineFx's code generation pipeline
- Uses `Frame` system for runtime code generation

### CleanResult Integration  
- Works with `Result` and `Result<T>` types
- Calls `IsError()` method for error detection
- Accesses `ErrorValue` and `Value` properties
- Maintains Result pattern semantics