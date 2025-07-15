namespace Identity.API.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var method = context.Request.Method;
        var path = context.Request.Path;

        // Filter out non-essential logs
        if (ShouldSkipLogging(method, path))
        {
            await _next(context);
            return;
        }

        var traceId = context.TraceIdentifier;
        var userId = context.User?.FindFirst("uid")?.Value ?? "anonymous";
        var ipAddress = context.Connection.RemoteIpAddress?.ToString();

        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["UserId"] = userId,
            ["IpAddress"] = ipAddress
        }))
        {
            try
            {
                _logger.LogInformation("Handling request {Method} {Path} from IP {IpAddress}", method, path, ipAddress);
                await _next(context);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred. IP: {IpAddress}", ipAddress);
                throw; // Re-throw the exception for global exception handling
            }
        }
    }

    private static bool ShouldSkipLogging(string method, PathString path)
    {
        // Ignore swagger, health, favicon, etc.
        if (path.StartsWithSegments("/swagger") ||
            path.StartsWithSegments("/health") ||
            path.StartsWithSegments("/favicon.ico"))
            return true;

        return false;
    }
}
