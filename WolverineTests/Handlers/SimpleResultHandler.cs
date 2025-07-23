using CleanResult;

namespace WolverineTests.Handlers;

/// <summary>
/// Handler demonstrating simple Result (non-generic) continuation strategy
/// LoadAsync returns Result, Handle method receives no extracted value
/// </summary>
public class SimpleResultHandler
{
    // Load method that returns simple Result
    public static async Task<Result> LoadAsync(SimpleCommand command)
    {
        // Simulate validation - fail for negative IDs
        if (command.Id < 0)
            return Result.Error("Invalid ID", 400);

        // Simulate not found - fail for ID = 999
        if (command.Id == 999)
            return Result.Error("Entity not found", 404);

        // Success case
        return Result.Ok();
    }

    // Handle method - gets called only if LoadAsync succeeds
    public static async Task<Result<string>> Handle(SimpleCommand command)
    {
        // This will only execute if LoadAsync returned success
        return Result.Ok($"Processed command with ID: {command.Id}");
    }
}