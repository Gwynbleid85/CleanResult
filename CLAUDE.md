# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

CleanResult is a .NET 8.0 library providing Rust-inspired Result types for functional error handling in C# applications. The solution contains four main projects:

- **CleanResult** - Core library with Result<T> and Result types
- **CleanResult.WolverineFx** - Integration with WolverineFx messaging framework  
- **CleanResult.Swashbuckle** - Swagger/OpenAPI integration
- **Tests** - Unit tests using xUnit

## Development Commands

### Build and Test
```bash
# Build the entire solution
dotnet build CleanResult.sln

# Run tests
dotnet test Tests/Tests.csproj

# Build specific project
dotnet build CleanResult/CleanResult.csproj

# Build in release mode (generates NuGet packages)
dotnet build -c Release
```

### Package Management
```bash
# Restore NuGet packages
dotnet restore

# Pack specific project for NuGet
dotnet pack CleanResult/CleanResult.csproj -c Release
```

## Architecture

### Core Result Pattern
The library implements a Rust-inspired Result pattern with two main types:
- `Result` - Non-generic result for operations without return values
- `Result<T>` - Generic result for operations that return values

Both types implement `IResult` for direct ASP.NET Core controller integration and support implicit conversion between error states.

### Error Handling
Error structure follows RFC 9457 (Problem Details for HTTP APIs) with these properties:
- `Type` - URI reference identifying the problem type
- `Title` - Human-readable summary
- `Status` - HTTP status code  
- `Detail` - Specific explanation
- `Instance` - URI identifying specific occurrence
- `Errors` - Additional validation problems

### Key Implementation Files
- `CleanResult/SimpleResult.cs` - Non-generic Result implementation
- `CleanResult/ValueResult.cs` - Generic Result<T> implementation  
- `CleanResult/Error.cs` - Error structure following RFC 9457
- `CleanResult.WolverineFx/CleanResultContinuationStrategy.cs` - WolverineFx integration

### Project Structure
Each project follows standard .NET conventions:
- Auto-generates NuGet packages on build (`GeneratePackageOnBuild = true`)
- Uses semantic versioning (currently 1.2.2)
- Includes nullable reference types and implicit usings
- Targets .NET 8.0

### Testing Strategy
- Uses xUnit testing framework
- Tests cover serialization, interfaces, and core Result functionality
- No specific build targets for test coverage reporting detected

## Integration Patterns

### ASP.NET Core
Result types implement `IResult` allowing direct return from controllers:
```csharp
[HttpGet("{id}")]
public Result<User> GetUser(int id) => _service.GetUserById(id);
```

### WolverineFx Messaging
The continuation strategy automatically handles Result types in message handlers:
- Extracts success values for handler method parameters
- Short-circuits on errors and returns error Result
- Supports both Load/LoadAsync patterns

Success results return HTTP 200 (with value) or 204 (without value). Error results use the error status code and return JSON problem details.