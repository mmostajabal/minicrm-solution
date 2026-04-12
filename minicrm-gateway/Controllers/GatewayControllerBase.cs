using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Text.Json;
using System.Web;

namespace MiniCRM.Gateway.Controllers;

/// <summary>
/// Base class for all gateway controllers. Provides typed forward helpers
/// (GET / POST / PUT / PATCH) that proxy to a named downstream service.
/// </summary>
public abstract class GatewayControllerBase : ControllerBase
{
    private readonly HttpClient _client;
    protected readonly ILogger _logger;

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    protected GatewayControllerBase(
        IHttpClientFactory factory,
        string serviceName,
        IConfiguration config,
        ILogger logger)
    {
        _client = factory.CreateClient(serviceName);
        _logger = logger;

        // Set base address from config
        var baseUrl = config[$"Services:{serviceName}"]
            ?? throw new InvalidOperationException($"Services:{serviceName} is not configured.");
        _client.BaseAddress = new Uri(baseUrl);
    }

    protected async Task<IActionResult> ForwardGetAsync(string path, CancellationToken ct)
    {
        _logger.LogDebug("GET {Path}", path);
        var response = await _client.GetAsync(path, ct);
        return await BuildResult(response, ct);
    }

    protected async Task<IActionResult> ForwardPostAsync<T>(string path, T body, CancellationToken ct)
    {
        _logger.LogDebug("POST {Path}", path);
        var response = await _client.PostAsJsonAsync(path, body, JsonOpts, ct);
        return await BuildResult(response, ct);
    }

    protected async Task<IActionResult> ForwardPutAsync<T>(string path, T body, CancellationToken ct)
    {
        _logger.LogDebug("PUT {Path}", path);
        var response = await _client.PutAsJsonAsync(path, body, JsonOpts, ct);
        return await BuildResult(response, ct);
    }

    protected async Task<IActionResult> ForwardPatchAsync<T>(string path, T body, CancellationToken ct)
    {
        _logger.LogDebug("PATCH {Path}", path);
        var response = await _client.PatchAsJsonAsync(path, body, JsonOpts, ct);
        return await BuildResult(response, ct);
    }

    private async Task<IActionResult> BuildResult(HttpResponseMessage response, CancellationToken ct)
    {
        var content = await response.Content.ReadAsStringAsync(ct);
        if (content.Length == 0)
            return StatusCode((int)response.StatusCode);

        try
        {
            return StatusCode((int)response.StatusCode, JsonSerializer.Deserialize<object>(content));
        }
        catch
        {
            _logger.LogWarning("Non-JSON response ({Status}): {Content}", (int)response.StatusCode, content);
            return StatusCode((int)response.StatusCode, new { error = content });
        }
    }

    /// <summary>Converts an object's public properties to a URL query string.</summary>
    protected static string QueryFrom(object? obj)
    {
        if (obj is null) return string.Empty;
        var query = HttpUtility.ParseQueryString(string.Empty);
        foreach (var prop in obj.GetType().GetProperties())
        {
            var val = prop.GetValue(obj);
            if (val is not null)
                query[prop.Name] = val.ToString();
        }
        return query.ToString() ?? string.Empty;
    }
}
