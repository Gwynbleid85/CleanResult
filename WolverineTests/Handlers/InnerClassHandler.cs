using CleanResult;

namespace WolverineTests.Handlers;

public record InnerCommand(int Id);

public class InnerClassHandler
{
// Load method that returns simple Result
    public static async Task<Result<InnerData>> LoadAsync(InnerCommand command)
    {
        // Simulate validation - fail for negative IDs
        if (command.Id < 0)
            return Result.Error("Invalid ID", 400);

        // Simulate not found - fail for ID = 999
        if (command.Id == 999)
            return Result.Error("Entity not found", 404);

        // Success case
        return Result.Ok(new InnerData { Id = 42, Name = "Inner Data", Email = "asdf@asdf.asdf" });
    }

// Handle method - gets called only if LoadAsync succeeds
    public static async Task<Result<User>> Handle(InnerCommand command, InnerData innerData)
    {
        // This will only execute if LoadAsync returned success
        return Result.Ok(
            new User
            {
                Id = innerData.Id,
                Name = innerData.Name,
                Email = innerData.Email
            });
    }

    public class InnerData
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Email { get; set; }
    }
}