namespace CleanResult.Swashbuckle.Tests.Handlers;

public record CascadingTupleCommand(bool Fail);

public class CascadingTupleHandler
{
    public static async Task<(Result<int>, string?)> LoadAsync(CascadingTupleCommand command)
    {
        if (command.Fail)
        {
            return (Result.Error("Testing error", 417), null);
        }
        return (Result.Ok(1), "asdf");
    }

    public static Task<(Result<string> result, string? message, int? value)> Handle(
        CascadingTupleCommand command,
        int loadAsyncSuccessValue
    )
    {
        return Task.FromResult<(Result<string> result, string? message, int? value)>(
            (Result.Ok(loadAsyncSuccessValue.ToString()), "loadAsyncSuccessValue", 1)
        );
    }
}
