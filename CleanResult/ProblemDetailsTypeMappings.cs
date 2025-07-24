namespace CleanResult;

public class ProblemDetailsTypeMappings
{
    public static string GetProblemType(int statusCode)
    {
        return statusCode switch
        {
            // 1xx Informational
            100 => "https://tools.ietf.org/html/rfc7231#section-6.2.1",
            101 => "https://tools.ietf.org/html/rfc7231#section-6.2.2",

            // 2xx Success
            200 => "https://tools.ietf.org/html/rfc7231#section-6.3.1",
            201 => "https://tools.ietf.org/html/rfc7231#section-6.3.2",
            202 => "https://tools.ietf.org/html/rfc7231#section-6.3.3",
            203 => "https://tools.ietf.org/html/rfc7231#section-6.3.4",
            204 => "https://tools.ietf.org/html/rfc7231#section-6.3.5",
            205 => "https://tools.ietf.org/html/rfc7231#section-6.3.6",
            206 => "https://tools.ietf.org/html/rfc7233#section-4.1",

            // 3xx Redirection
            300 => "https://tools.ietf.org/html/rfc7231#section-6.4.1",
            301 => "https://tools.ietf.org/html/rfc7231#section-6.4.2",
            302 => "https://tools.ietf.org/html/rfc7231#section-6.4.3",
            303 => "https://tools.ietf.org/html/rfc7231#section-6.4.4",
            304 => "https://tools.ietf.org/html/rfc7232#section-4.1",
            305 => "https://tools.ietf.org/html/rfc7231#section-6.4.5",
            307 => "https://tools.ietf.org/html/rfc7231#section-6.4.7",
            308 => "https://tools.ietf.org/html/rfc7538#section-3",

            // 4xx Client Errors
            400 => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            401 => "https://tools.ietf.org/html/rfc7235#section-3.1",
            402 => "https://tools.ietf.org/html/rfc7231#section-6.5.2",
            403 => "https://tools.ietf.org/html/rfc7231#section-6.5.3",
            404 => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            405 => "https://tools.ietf.org/html/rfc7231#section-6.5.5",
            406 => "https://tools.ietf.org/html/rfc7231#section-6.5.6",
            407 => "https://tools.ietf.org/html/rfc7235#section-3.2",
            408 => "https://tools.ietf.org/html/rfc7231#section-6.5.7",
            409 => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
            410 => "https://tools.ietf.org/html/rfc7231#section-6.5.9",
            411 => "https://tools.ietf.org/html/rfc7231#section-6.5.10",
            412 => "https://tools.ietf.org/html/rfc7232#section-4.2",
            413 => "https://tools.ietf.org/html/rfc7231#section-6.5.11",
            414 => "https://tools.ietf.org/html/rfc7231#section-6.5.12",
            415 => "https://tools.ietf.org/html/rfc7231#section-6.5.13",
            416 => "https://tools.ietf.org/html/rfc7233#section-4.4",
            417 => "https://tools.ietf.org/html/rfc7231#section-6.5.14",
            418 => "https://tools.ietf.org/html/rfc2324#section-2.3.2",
            421 => "https://tools.ietf.org/html/rfc7540#section-9.1.2",
            422 => "https://tools.ietf.org/html/rfc4918#section-11.2",
            423 => "https://tools.ietf.org/html/rfc4918#section-11.3",
            424 => "https://tools.ietf.org/html/rfc4918#section-11.4",
            425 => "https://tools.ietf.org/html/rfc8470#section-5.2",
            426 => "https://tools.ietf.org/html/rfc7231#section-6.5.15",
            428 => "https://tools.ietf.org/html/rfc6585#section-3",
            429 => "https://tools.ietf.org/html/rfc6585#section-4",
            431 => "https://tools.ietf.org/html/rfc6585#section-5",
            451 => "https://tools.ietf.org/html/rfc7725#section-3",

            // 5xx Server Errors
            500 => "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            501 => "https://tools.ietf.org/html/rfc7231#section-6.6.2",
            502 => "https://tools.ietf.org/html/rfc7231#section-6.6.3",
            503 => "https://tools.ietf.org/html/rfc7231#section-6.6.4",
            504 => "https://tools.ietf.org/html/rfc7231#section-6.6.5",
            505 => "https://tools.ietf.org/html/rfc7231#section-6.6.6",
            506 => "https://tools.ietf.org/html/rfc2295#section-8.1",
            507 => "https://tools.ietf.org/html/rfc4918#section-11.5",
            508 => "https://tools.ietf.org/html/rfc5842#section-7.2",
            510 => "https://tools.ietf.org/html/rfc2774#section-7",
            511 => "https://tools.ietf.org/html/rfc6585#section-6",

            // Default fallback
            _ => "https://tools.ietf.org/html/rfc7231"
        };
    }
}