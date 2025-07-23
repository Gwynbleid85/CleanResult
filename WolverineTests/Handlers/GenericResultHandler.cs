using CleanResult;

namespace WolverineTests.Handlers;

/// <summary>
/// Handler demonstrating generic Result
/// <T>
/// continuation strategy
/// LoadAsync returns Result<User>, Handle method receives extracted User
/// </summary>
public class GenericResultHandler
{
    // Load method that returns Result<User>
    public static async Task<Result<User>> LoadAsync(GenericCommand command)
    {
        // Find user by ID
        var user = DataStore.Users.FirstOrDefault(u => u.Id == command.Id);

        if (user == null)
            return Result.Error("User not found", 404);

        return Result.Ok(user);
    }

    // Handle method - receives the extracted User from LoadAsync success
    public static async Task<Result<UserDto>> Handle(GenericCommand command, User user)
    {
        // Transform entity to DTO
        var userDto = new UserDto(user.Id, user.Name, user.Email);

        return Result.Ok(userDto);
    }
}