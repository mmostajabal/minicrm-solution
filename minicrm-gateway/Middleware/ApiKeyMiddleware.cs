namespace MiniCRM.Gateway.Middleware;

/// <summary>
/// Validates the X-Gateway-Key header on every inbound request.
/// The key must match Gateway:GatewayApiKey in appsettings.json.
/// </summary>
public class ApiKeyMiddleware
{
    private const string HeaderName = "X-Gateway-Key";
    private readonly RequestDelegate _next;
    private readonly string _expectedKey;
    private readonly ILogger<ApiKeyMiddleware> _logger;

    public ApiKeyMiddleware(RequestDelegate next, IConfiguration config, ILogger<ApiKeyMiddleware> logger)
    {
        _next = next;
        _expectedKey = config["Gateway:GatewayApiKey"]
            ?? throw new InvalidOperationException("Gateway:GatewayApiKey is not configured.");
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Always allow Swagger UI in development
        if (context.Request.Path.StartsWithSegments("/swagger"))
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(HeaderName, out var providedKey)
            || providedKey != _expectedKey)
        {
            _logger.LogWarning("Unauthorized request from {IP} – missing or invalid {Header}",
                context.Connection.RemoteIpAddress, HeaderName);
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new { error = "Unauthorized: invalid or missing gateway API key." });
            return;
        }

        await _next(context);
    }
}
