using CleanResult;

namespace WolverineTests.Handlers;

/// <summary>
/// Handler demonstrating Result
/// <(User, Product)>
/// tuple continuation strategy
/// LoadAsync returns Result<(User, Product)>, Handle method receives extracted tuple items
/// </summary>
public class TupleResultHandler
{
    // Load method that returns Result with tuple of (User, Product)
    public static Task<Result<(User user, Product product)>> LoadAsync(TupleCommand command)
    {
        // Find user and product by ID (using same ID for simplicity)
        var user = DataStore.Users.FirstOrDefault(u => u.Id == command.Id);
        var product = DataStore.Products.FirstOrDefault(p => p.Id == command.Id);

        if (user == null)
            return Task.FromResult(Result<(User, Product)>.Error("User not found", 404));

        if (product == null)
            return Task.FromResult(Result<(User, Product)>.Error("Product not found", 404));

        return Task.FromResult(Result.Ok((user, product)));
    }

    // Handle method - should receive extracted tuple items as separate parameters
    // The continuation strategy should extract tuple.Item1 and tuple.Item2
    public static Task<Result<OrderDto>> Handle(TupleCommand command,
        (User user, Product product) loadAsyncSuccessValue)
    {
        // Create order DTO from the extracted user and product
        var orderDto = new OrderDto(
            Random.Shared.Next(1000, 9999),
            loadAsyncSuccessValue.user.Id,
            loadAsyncSuccessValue.product.Id,
            1
        );

        return Task.FromResult(Result.Ok(orderDto));
    }
}

/// <summary>
/// Alternative handler showing different tuple access patterns
/// </summary>
public class AlternativeTupleHandler
{
    // Load method returning Result<(int, string, bool)>
    public static async Task<Result<(int count, string message, bool isValid)>> LoadAsync(AlternativeTupleCommand command)
    {
        await Task.Delay(5);

        if (command.Id <= 0)
            return Result<(int, string, bool)>.Error("Invalid command ID", 400);

        return Result.Ok((command.Id * 2, $"Message for {command.Id}", command.Id % 2 == 0));
    }

    // Handle method receiving the tuple value
    public static async Task<Result<string>> Handle(AlternativeTupleCommand command,
        (int count, string message, bool isValid) loadAsyncSuccessValue)
    {
        await Task.Delay(5);

        var result =
            $"Count: {loadAsyncSuccessValue.count}, Message: {loadAsyncSuccessValue.message}, Valid: {loadAsyncSuccessValue.isValid}";
        return Result.Ok(result);
    }
}