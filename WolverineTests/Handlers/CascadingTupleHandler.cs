using CleanResult;

namespace WolverineTests.Handlers;

public class CascadingTupleHandler
{
    public static Task<Result<int>> LoadAsync(CascadingTupleCommand command)
    {
        return command.Id > 0
            ? Task.FromResult(Result.Ok(command.Id))
            : Task.FromResult(Result<int>.Error("Invalid command ID", 400));
    }

    public static Task<(Result<int> result, string? message, int? value)> Handle(
        CascadingTupleCommand command,
        int loadAsyncSuccessValue)
    {
        string? message = $"Value: {loadAsyncSuccessValue}";
        return Task.FromResult<(Result<int> result, string? message, int? value)>((
            Result.Ok(loadAsyncSuccessValue),
            message,
            loadAsyncSuccessValue));
    }
}
