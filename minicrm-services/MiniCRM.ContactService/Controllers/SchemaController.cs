using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using MiniCRM.Shared.DTOs;
using MiniCRM.Shared.Exceptions;

namespace MiniCRM.ContactService.Controllers;

/// <summary>
/// Exposes schema/metadata endpoints (categories, statuses, custom fields).
/// Hosted in ContactService since schema relates to CRM structure.
/// </summary>
[ApiController]
[Route("api/schema")]
[Produces("application/json")]
public class SchemaController : ControllerBase
{
    private readonly SchemaApiClient _api;
    private readonly ILogger<SchemaController> _logger;

    public SchemaController(SchemaApiClient api, ILogger<SchemaController> logger)
    {
        _api    = api;
        _logger = logger;
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories(CancellationToken ct)
    {
        try   { return Ok(await _api.GetCategoriesAsync(ct)); }
        catch (MiniCrmException ex) { return StatusCode(ex.StatusCode ?? 500, new { error = ex.Message }); }
    }

    [HttpGet("{type}")]
    public async Task<IActionResult> GetSchema(string type, CancellationToken ct)
    {
        try   { return Ok(await _api.GetSchemaAsync(type, ct)); }
        catch (MiniCrmException ex) { return StatusCode(ex.StatusCode ?? 500, new { error = ex.Message }); }
    }
}

public class SchemaApiClient : MiniCrmClientBase
{
    private static readonly JsonSerializerOptions Opts = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public SchemaApiClient(HttpClient http, IConfiguration config, ILogger<SchemaApiClient> logger)
        : base(http, config, logger) { }

    public async Task<object> GetCategoriesAsync(CancellationToken ct)
    {
        await AcquireRateLimitAsync(ct);
        Logger.LogInformation("GET /Api/R3/Category");
        var response = await Http.GetAsync("/Api/R3/Category", ct);
        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<object>(Opts, ct) ?? new { };
    }

    public async Task<object> GetSchemaAsync(string type, CancellationToken ct)
    {
        await AcquireRateLimitAsync(ct);
        Logger.LogInformation("GET /Api/R3/Schema/{Type}", type);
        var response = await Http.GetAsync($"/Api/R3/Schema/{type}", ct);
        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<object>(Opts, ct) ?? new { };
    }
}
