namespace CleanResult.Swashbuckle.Tests.Handlers;

public record CascadingTupleCommand(int Id);

public class CascadingTupleHandler
{
    public static Task<(Result<string>, string)> LoadAsync(CascadingTupleCommand command)
    {
        return Task.FromResult((Result.Ok("asd"), "asdf"));
    }

    public static Task<(Result<string> result, string? message, int? value)> Handle(
        CascadingTupleCommand command,
        string loadAsyncSuccessValue
    )
    {
        var message = $"Value: {loadAsyncSuccessValue}";
        return Task.FromResult<(Result<string> result, string? message, int? value)>(
            (Result.Error(), "message", 1)
        );
    }
}
