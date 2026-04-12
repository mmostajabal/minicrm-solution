using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using MiniCRM.Shared.DTOs;
using MiniCRM.Shared.Exceptions;

namespace MiniCRM.InvoiceService.Controllers;

/// <summary>
/// Read-only invoice endpoint. No write operations per spec.
/// </summary>
[ApiController]
[Route("api/invoices")]
[Produces("application/json")]
public class InvoicesController : ControllerBase
{
    private readonly InvoiceApiClient _api;
    private readonly ILogger<InvoicesController> _logger;

    public InvoicesController(InvoiceApiClient api, ILogger<InvoicesController> logger)
    {
        _api    = api;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Query([FromQuery] InvoiceSearchParams p, CancellationToken ct)
    {
        try   { return Ok(await _api.QueryAsync(p, ct)); }
        catch (MiniCrmException ex) { return StatusCode(ex.StatusCode ?? 500, new { error = ex.Message }); }
    }
}

// --- miniCRM API client ---

public class InvoiceApiClient : MiniCrmClientBase
{
    private static readonly JsonSerializerOptions Opts = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public InvoiceApiClient(HttpClient http, IConfiguration config, ILogger<InvoiceApiClient> logger)
        : base(http, config, logger) { }

    public async Task<InvoiceListResponse> QueryAsync(InvoiceSearchParams p, CancellationToken ct)
    {
        await AcquireRateLimitAsync(ct);
        var qs = BuildQuery(p);
        Logger.LogInformation("GET /Api/Invoice?{Query}", qs);
        var response = await Http.GetAsync($"/Api/Invoice?{qs}", ct);
        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<InvoiceListResponse>(Opts, ct)
               ?? new InvoiceListResponse();
    }

    private static string BuildQuery(InvoiceSearchParams p)
    {
        var q = HttpUtility.ParseQueryString(string.Empty);
        if (p.ProjectId.HasValue) q["ProjectId"] = p.ProjectId.ToString();
        if (p.ContactId.HasValue) q["ContactId"] = p.ContactId.ToString();
        if (!string.IsNullOrWhiteSpace(p.Status)) q["Status"] = p.Status;
        q["Page"]        = p.Page.ToString();
        q["MaxElements"] = p.MaxElements.ToString();
        return q.ToString()!;
    }
}
