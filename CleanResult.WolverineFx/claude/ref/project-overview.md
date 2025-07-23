# CleanResult.WolverineFx - Project Overview

## Summary

CleanResult.WolverineFx is a .NET 8.0 extension library that provides seamless integration between the [CleanResult](https://github.com/Gwynbleid85/CleanResult) pattern library and the [WolverineFx](https://wolverine.netlify.app/) messaging framework. It enables automatic handling of Result types in message handlers through a custom continuation strategy.

## Key Features

- **Automatic Error Handling**: Eliminates boilerplate error handling code in message handlers
- **Type-Safe Value Extraction**: Automatically extracts success values from `Result<T>` types
- **Dependency Injection Integration**: Makes extracted values available via DI container
- **Code Generation**: Uses runtime code generation for optimal performance
- **Convention-Based**: Works with standard Load/LoadAsync method naming patterns

## Architecture

The library follows these architectural principles:

1. **Extension Pattern**: Extends WolverineFx without modifying the core framework
2. **Convention-Based**: Uses Load/LoadAsync naming conventions for consistency
3. **Code Generation**: Leverages runtime code generation for high performance
4. **Type Safety**: Maintains strong typing throughout the message handling pipeline
5. **Separation of Concerns**: Separates data loading logic from business logic
6. **Error Handling**: Provides consistent, automatic error handling patterns

## Project Structure

```
CleanResult.WolverineFx/
├── CleanResult.WolverineFx.csproj    # Project configuration and dependencies
├── CleanResultContinuationStrategy.cs # Main implementation
├── README.md                         # Usage documentation
└── claude/ref/                       # Reference documentation
```

## Dependencies

- **CleanResult (v1.2.2)**: Core Result pattern library providing `Result` and `Result<T>` types
- **WolverineFx (v4.5.2)**: Messaging framework being extended with Result pattern support

## Target Audience

This library is designed for developers who:
- Use WolverineFx for messaging in their .NET applications
- Want to implement consistent error handling patterns
- Prefer the Result pattern over exception-based error handling
- Need clean separation between data loading and business logic