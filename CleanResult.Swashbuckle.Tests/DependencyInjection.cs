using System.Reflection;
using CleanResult.WolverineFx;
using Microsoft.OpenApi;
using Wolverine;
using Wolverine.Middleware;

namespace CleanResult.Swashbuckle.Tests;

/// <summary>
/// Class containing methods for registering shared services or utils and configuration shared behaviour.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Configure parts the project that need access to all assemblies (e.g. Wolverine)
    /// </summary>
    /// <param name="host"></param>
    /// <param name="assemblies"></param>
    /// <returns></returns>
    public static IHostBuilder AddProjects(this IHostBuilder host, string[] assemblies)
    {
        host.UseWolverine(opts => {
            foreach (var assembly in assemblies)
                opts.Discovery.IncludeAssembly(Assembly.Load(assembly));

            opts.Policies.AutoApplyTransactions();
            opts.Policies.UseDurableLocalQueues();
            opts.CodeGeneration.AddContinuationStrategy<CleanResultContinuationStrategy>();
        });

        return host;
    }

    /// <summary>
    /// Configure swagger
    /// </summary>
    /// <param name="services"></param>
    /// <param name="title"> Title of the OpenApi schema</param>
    /// <param name="assemblies"></param>
    /// <returns></returns>
    public static IServiceCollection AddSwagger(this IServiceCollection services, string title,
        string[] assemblies)
    {
        services.AddSwaggerGen(options => {
            // Configure basic swagger info 
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = title,
                Version = "v1",
                Description = "CleanResult API"
            });

            // Add XML comments from all assemblies to swagger
            foreach (var assembly in assemblies)
            {
                var assemblyXmlPath = Path.Combine(AppContext.BaseDirectory, $"{assembly}.xml");
                options.IncludeXmlComments(assemblyXmlPath);
            }

            // Make all strings nullable by defaults
            options.SupportNonNullableReferenceTypes();
            options.SchemaFilter<MakeAllPropertiesRequiredFilter>();
            options.AddCleanResultFilters();
        });

        return services;
    }
}