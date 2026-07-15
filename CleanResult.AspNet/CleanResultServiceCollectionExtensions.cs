using Microsoft.Extensions.DependencyInjection;

namespace CleanResult.AspNet;

/// <summary>
/// Extension methods for configuring CleanResult in ASP.NET Core applications.
/// </summary>
public static class CleanResultServiceCollectionExtensions
{
    /// <summary>
    /// Configures the global CleanResult options used by <see cref="Result" /> and <see cref="Result{T}" />.
    /// </summary>
    /// <param name="services">The service collection for the current application.</param>
    /// <param name="configure">A delegate that updates <see cref="CleanResultConfiguration.Options" />.</param>
    /// <returns>The same <see cref="IServiceCollection" /> instance so additional calls can be chained.</returns>
    public static IServiceCollection AddCleanResult(
        this IServiceCollection services,
        Action<CleanResultOptions> configure
    )
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        configure(CleanResultConfiguration.Options);
        return services;
    }
}
