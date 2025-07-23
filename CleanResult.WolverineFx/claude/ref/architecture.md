# Architecture and Design Decisions

## Overall Architecture

### Extension Pattern
The library follows an **extension pattern** rather than modification:
- Extends WolverineFx without changing the core framework
- Uses WolverineFx's plugin architecture (`IContinuationStrategy`)
- Maintains compatibility with existing WolverineFx features

### Code Generation Architecture
- Leverages WolverineFx's runtime code generation system
- Uses `Frame`-based code generation for optimal performance  
- Generates specialized code for each Result type scenario
- Avoids reflection overhead at runtime

## Key Design Decisions

### 1. Convention-Based Approach
**Decision**: Use method naming conventions (`Load`/`LoadAsync`) instead of attributes.

**Rationale**:
- Reduces boilerplate code
- Maintains clean, readable handlers
- Follows established patterns in the ecosystem
- Easier to understand and maintain

**Location**: `CleanResultContinuationStrategy.cs:75`

### 2. Frame-Based Code Generation
**Decision**: Use WolverineFx's Frame system for code generation.

**Rationale**:
- Optimal runtime performance (no reflection)
- Integrates seamlessly with WolverineFx pipeline
- Type-safe code generation
- Debugging support through generated code

**Implementation**: Two frame types for different Result scenarios
- `MaybeEndHandlerWithCleanResultFrame` for `Result`
- `MaybeEndHandlerWithGenericCleanResultFrame` for `Result<T>`

### 3. Automatic Value Extraction
**Decision**: Extract success values from `Result<T>` and inject them via DI.

**Rationale**:
- Eliminates repetitive `.Value` access in handlers
- Makes extracted values available to all handler dependencies
- Maintains type safety throughout the pipeline
- Reduces boilerplate code in business logic

**Location**: `CleanResultContinuationStrategy.cs:131`

### 4. Error-First Flow Control
**Decision**: Check for errors first and short-circuit on failure.

**Rationale**:
- Follows fail-fast principle
- Prevents business logic execution on invalid states
- Maintains consistent error handling patterns
- Reduces cognitive load in handlers

**Generated Code Pattern**:
```csharp
if (result.IsError())
{
    // Handle error and return early
    return;
}
// Continue with success path
```

### 5. Type Preservation and Conversion
**Decision**: Maintain proper type information and convert when necessary.

**Rationale**:
- Ensures type safety across handler boundaries
- Supports different return types between Load and Handle methods
- Enables flexible handler design patterns
- Maintains compatibility with existing code

**Location**: `CleanResultContinuationStrategy.cs:107`

## Component Architecture

### Core Components

```
CleanResultContinuationStrategy
├── TryFindContinuationHandler     # Entry point, type detection
├── GetFriendlyTypeName           # Type name conversion utility
├── GetHandlerReturnType          # Handler introspection
├── MaybeEndHandlerWithCleanResultFrame      # Non-generic Result handling
└── MaybeEndHandlerWithGenericCleanResultFrame  # Generic Result<T> handling
```

### Integration Points

```
WolverineFx Framework
├── IContinuationStrategy Interface
├── Code Generation Pipeline
├── Frame System
├── Variable Management
└── Dependency Injection Container

CleanResult Library
├── Result Type
├── Result<T> Type
├── IsError() Method
├── ErrorValue Property
└── Value Property
```

## Performance Considerations

### Runtime Performance
- **Code Generation**: Eliminates reflection overhead at runtime
- **Early Returns**: Short-circuits execution on errors
- **Direct Variable Access**: No dictionary lookups for extracted values
- **ConfigureAwait(false)**: Optimizes async operations

### Build-Time Performance
- **Type Caching**: Types are resolved once during code generation
- **Minimal Overhead**: Small impact on application startup
- **Efficient Frame Creation**: Lightweight frame objects

## Error Handling Architecture

### Error Propagation Flow
1. **Detection**: `IsError()` check in generated code
2. **Extraction**: Access `ErrorValue` property
3. **Type Conversion**: Convert to appropriate handler return type if needed
4. **Propagation**: Use `context.EnqueueCascadingAsync()` for error responses
5. **Termination**: Early return to prevent business logic execution

### Error Context Preservation
- Maintains original error information
- Preserves HTTP status codes and error messages
- Supports custom error types
- Enables error transformation between handler layers

## Dependency Injection Integration

### Variable Registration
**Location**: `CleanResultContinuationStrategy.cs:131`

```csharp
creates.Add(new Variable(result.VariableType.GetGenericArguments()[0], result.Usage + "SuccessValue"));
```

### Injection Mechanism
- Success values become first-class DI variables
- Available to all handler method parameters
- Maintains proper type information
- Follows standard DI resolution patterns

## Extensibility Points

### Future Extensions
The architecture supports potential extensions for:
- Custom error handling strategies
- Additional Result-like types
- Different naming conventions
- Custom value transformation pipelines

### Plugin Architecture
- Implements standard WolverineFx interfaces
- Can coexist with other continuation strategies
- Supports selective application to specific handlers
- Maintains framework neutrality

## Trade-offs and Limitations

### Trade-offs Made
1. **Convention over Configuration**: Less flexibility but more consistency
2. **Code Generation**: Better performance but more complex debugging
3. **Method Naming**: Simpler usage but requires naming discipline
4. **Automatic DI**: Less explicit but more convenient

### Current Limitations
1. **Method Naming**: Must use Load/LoadAsync convention
2. **Single Strategy**: One continuation strategy per Result type
3. **WolverineFx Dependency**: Tightly coupled to WolverineFx framework
4. **Testing**: Primarily designed for `InvokeAsync` usage patterns

### Design Constraints
- Must work within WolverineFx's code generation system
- Cannot modify CleanResult library interfaces
- Must maintain backward compatibility
- Performance cannot degrade compared to manual error handling