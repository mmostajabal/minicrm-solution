using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using MiniCRM.Shared.DTOs;
using MiniCRM.Shared.Exceptions;

namespace MiniCRM.ProjectService.Controllers;

[ApiController]
[Route("api/projects")]
[Produces("application/json")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectApiClient _api;
    private readonly ILogger<ProjectsController> _logger;

    public ProjectsController(IProjectApiClient api, ILogger<ProjectsController> logger)
    {
        _api    = api;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] ProjectSearchParams p, CancellationToken ct)
    {
        try   { return Ok(await _api.SearchAsync(p, ct)); }
        catch (MiniCrmException ex) { return StatusCode(ex.StatusCode ?? 500, new { error = ex.Message }); }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken ct)
    {
        try   { return Ok(await _api.GetByIdAsync(id, ct)); }
        catch (MiniCrmNotFoundException) { return NotFound(new { error = $"Project {id} not found." }); }
        catch (MiniCrmException ex)      { return StatusCode(ex.StatusCode ?? 500, new { error = ex.Message }); }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProjectDto dto, CancellationToken ct)
    {
        try
        {
            var result = await _api.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }
        catch (MiniCrmException ex) { return StatusCode(ex.StatusCode ?? 500, new { error = ex.Message }); }
    }

    /// <summary>
    /// Change ONLY the status of a project. Per spec: projekt_statusz_valtas
    /// must not modify any other field.
    /// </summary>
    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> ChangeStatus(int id, [FromBody] ProjectStatusChangeRequest req, CancellationToken ct)
    {
        try   { return Ok(await _api.ChangeStatusAsync(id, req.StatusId, ct)); }
        catch (MiniCrmNotFoundException) { return NotFound(new { error = $"Project {id} not found." }); }
        catch (MiniCrmException ex)      { return StatusCode(ex.StatusCode ?? 500, new { error = ex.Message }); }
    }
}

// --- Interface ---

public interface IProjectApiClient
{
    Task<ProjectListResponse> SearchAsync(ProjectSearchParams p, CancellationToken ct);
    Task<ProjectDto> GetByIdAsync(int id, CancellationToken ct);
    Task<ProjectDto> CreateAsync(ProjectDto dto, CancellationToken ct);
    Task<ProjectDto> ChangeStatusAsync(int id, int statusId, CancellationToken ct);
}

// --- miniCRM API client ---

public class ProjectApiClient : MiniCrmClientBase, IProjectApiClient
{
    private static readonly JsonSerializerOptions Opts = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public ProjectApiClient(HttpClient http, IConfiguration config, ILogger<ProjectApiClient> logger)
        : base(http, config, logger) { }

    public async Task<ProjectListResponse> SearchAsync(ProjectSearchParams p, CancellationToken ct)
    {
        await AcquireRateLimitAsync(ct);
        var qs = BuildQuery(p);
        Logger.LogInformation("GET /Api/R3/Project?{Query}", qs);
        var response = await Http.GetAsync($"/Api/R3/Project?{qs}", ct);
        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<ProjectListResponse>(Opts, ct)
               ?? new ProjectListResponse();
    }

    public async Task<ProjectDto> GetByIdAsync(int id, CancellationToken ct)
    {
        await AcquireRateLimitAsync(ct);
        Logger.LogInformation("GET /Api/R3/Project/{Id}", id);
        var response = await Http.GetAsync($"/Api/R3/Project/{id}", ct);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            throw new MiniCrmNotFoundException("Project", id);
        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<ProjectDto>(Opts, ct)
               ?? throw new MiniCrmApiException("Empty response from Project GET", 502);
    }

    public async Task<ProjectDto> CreateAsync(ProjectDto dto, CancellationToken ct)
    {
        await AcquireRateLimitAsync(ct);
        Logger.LogInformation("PUT /Api/R3/Project (create)");
        var payload = JsonSerializer.Serialize(dto, Opts);
        var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await Http.PutAsync("/Api/R3/Project", content, ct);
        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<ProjectDto>(Opts, ct)
               ?? throw new MiniCrmApiException("Empty response from Project create", 502);
    }

    public async Task<ProjectDto> ChangeStatusAsync(int id, int statusId, CancellationToken ct)
    {
        await AcquireRateLimitAsync(ct);
        Logger.LogInformation("PUT /Api/R3/Project/{Id} → StatusId={StatusId}", id, statusId);
        // Only send StatusId – no other fields – as per spec (projekt_statusz_valtas)
        var payload = JsonSerializer.Serialize(new { StatusId = statusId }, Opts);
        var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await Http.PutAsync($"/Api/R3/Project/{id}", content, ct);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            throw new MiniCrmNotFoundException("Project", id);
        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<ProjectDto>(Opts, ct)
               ?? throw new MiniCrmApiException("Empty response from Project status change", 502);
    }

    private static string BuildQuery(ProjectSearchParams p)
    {
        var q = HttpUtility.ParseQueryString(string.Empty);
        if (p.CategoryId.HasValue)  q["CategoryId"]  = p.CategoryId.ToString();
        if (p.StatusId.HasValue)    q["StatusId"]     = p.StatusId.ToString();
        if (p.ContactId.HasValue)   q["ContactId"]    = p.ContactId.ToString();
        if (p.UserId.HasValue)      q["UserId"]       = p.UserId.ToString();
        if (!string.IsNullOrWhiteSpace(p.Name)) q["Name"] = p.Name;
        q["Page"]        = p.Page.ToString();
        q["MaxElements"] = p.MaxElements.ToString();
        return q.ToString()!;
    }
}
