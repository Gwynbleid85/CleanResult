using Microsoft.AspNetCore.Http;

namespace Tests.Utils;

public class HttpContextUtils
{
    public static HttpContext GetHttpContext()
    {
        var httpContext = new DefaultHttpContext();
        var memoryStream = new MemoryStream();
        httpContext.Response.Body = memoryStream;
        return httpContext;
    }

    public static string ReadContextBody(HttpContext httpContext)
    {
        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(httpContext.Response.Body);
        return reader.ReadToEnd();
    }
}