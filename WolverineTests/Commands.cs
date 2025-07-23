namespace WolverineTests;

// Simple commands for testing
public record SimpleCommand(int Id);

public record GenericCommand(int Id);

public record TupleCommand(int Id);

public record AlternativeTupleCommand(int Id);

// Response DTOs
public record UserDto(int Id, string Name, string Email);

public record ProductDto(int Id, string Name, decimal Price);

public record OrderDto(int Id, int UserId, int ProductId, int Quantity);

// Domain entities
public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

// Mock data store
public static class DataStore
{
    public static readonly List<User> Users = new()
    {
        new User { Id = 1, Name = "John Doe", Email = "john@example.com" },
        new User { Id = 2, Name = "Jane Smith", Email = "jane@example.com" }
    };

    public static readonly List<Product> Products = new()
    {
        new Product { Id = 1, Name = "Laptop", Price = 999.99m },
        new Product { Id = 2, Name = "Mouse", Price = 29.99m }
    };
}