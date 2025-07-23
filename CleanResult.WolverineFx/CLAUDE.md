# CleanResult.WolverineFx - Claude Reference

## Project Overview

CleanResult.WolverineFx is a .NET 8.0 extension library that integrates the CleanResult pattern with WolverineFx messaging framework, providing automatic error handling and value extraction for Result types.

## Key Information

- **Version**: 1.2.2
- **Target Framework**: .NET 8.0
- **Main Dependencies**: CleanResult (1.2.2), WolverineFx (4.5.2)
- **Main Implementation**: `CleanResultContinuationStrategy.cs`

## Documentation Reference

The complete implementation reference is available in the `./claude/ref/` directory:

### 📖 [Project Overview](./claude/ref/project-overview.md)
- Project summary and key features
- Architecture principles
- Target audience and use cases
- Dependencies overview

### 🏗️ [Core Implementation](./claude/ref/core-implementation.md)
- `CleanResultContinuationStrategy` class deep dive
- Method implementations and logic
- Frame classes for code generation
- Integration points with WolverineFx and CleanResult

### 🚀 [Usage Patterns](./claude/ref/usage-patterns.md)
- Registration and setup examples
- Handler patterns and conventions
- Error handling patterns
- Best practices and common scenarios

### 🏛️ [Architecture](./claude/ref/architecture.md)
- Design decisions and rationale
- Component architecture
- Performance considerations
- Trade-offs and limitations

### 📚 [API Reference](./claude/ref/api-reference.md)
- Complete API documentation
- Method signatures and parameters
- Generated code patterns
- Registration and configuration

## Quick Start

### Registration
```csharp
host.UseWolverine(opts =>
{
    opts.CodeGeneration.AddContinuationStrategy<CleanResultContinuationStrategy>();
});
```

### Handler Pattern
```csharp
public class MyHandler
{
    // Load method returns Result<T>
    public static async Task<Result<Entity>> LoadAsync(Command cmd, IQuerySession session)
    {
        var entity = await session.LoadAsync<Entity>(cmd.Id);
        return entity != null ? Result.Ok(entity) : Result.Error("Not found");
    }

    // Handle method receives extracted success value
    public static async Task<Result<ResponseDto>> Handle(Command cmd, Entity entity)
    {
        // Business logic with extracted entity
        return Result.Ok(new ResponseDto());
    }
}
```

## Development Notes

### Testing
- No specific test framework detected in this project
- Testing strategy primarily focused on InvokeAsync patterns
- Integration testing recommended with actual WolverineFx setup

### Build and Package
- Auto-generates NuGet package on build
- Includes README, LICENSE, and logo in package
- Uses semantic versioning (currently 1.2.2)

### Code Style
- Uses nullable reference types
- Implicit usings enabled
- Follows async/await best practices with ConfigureAwait(false)

## Important Files

- `CleanResult.WolverineFx.csproj` - Project configuration and dependencies
- `CleanResultContinuationStrategy.cs` - Main implementation (158 lines)
- `README.md` - Usage documentation and examples

## Keywords
`result pattern`, `error handling`, `wolverinefx`, `messaging`, `code generation`, `continuation strategy`