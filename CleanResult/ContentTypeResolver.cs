using System.Xml;
using System.Xml.Linq;

namespace CleanResult;

/// <summary>
/// Resolves the appropriate Content-Type header based on the type of the value.
/// </summary>
internal static class ContentTypeResolver
{
    /// <summary>
    /// Gets the appropriate Content-Type for a given value type.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <returns>The Content-Type string (e.g., "application/json", "text/plain").</returns>
    public static string GetContentType<T>()
    {
        var type = typeof(T);

        return type switch
        {
            // Handle string as plain text
            _ when type == typeof(string) => "text/plain; charset=utf-8",

            // Handle byte arrays as binary data
            _ when type == typeof(byte[]) => "application/octet-stream",

            // Handle streams as binary data
            _ when typeof(Stream).IsAssignableFrom(type) => "application/octet-stream",

            // Handle XML types
            _ when type == typeof(XmlDocument) ||
                   type == typeof(XDocument) ||
                   type == typeof(XElement) ||
                   typeof(XNode).IsAssignableFrom(type) => "application/xml; charset=utf-8",

            // Default to JSON for all other types (objects, primitives, etc.)
            _ => "application/json; charset=utf-8"
        };
    }

    /// <summary>
    /// Determines if a type should be serialized as JSON.
    /// </summary>
    /// <typeparam name="T">The type to check.</typeparam>
    /// <returns>True if the type should be JSON serialized, false otherwise.</returns>
    public static bool ShouldSerializeAsJson<T>()
    {
        var contentType = GetContentType<T>();
        return contentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase);
    }
}