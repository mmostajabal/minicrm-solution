using Microsoft.AspNetCore.Mvc;
using MiniCRM.Shared.DTOs;

namespace MiniCRM.Gateway.Controllers;

/// <summary>
/// Gateway proxy for Project operations → forwards to ProjectService (port 5002).
/// </summary>
[ApiController]
[Route("api/projects")]
[Produces("application/json")]
public class ProjectsController : GatewayControllerBase
{
    public ProjectsController(IHttpClientFactory factory, IConfiguration config, ILogger<ProjectsController> logger)
        : base(factory, "ProjectService", config, logger) { }

    /// <summary>Search projects.</summary>
    [HttpGet]
    public Task<IActionResult> Search([FromQuery] ProjectSearchParams p, CancellationToken ct)
        => ForwardGetAsync($"/api/projects?{QueryFrom(p)}", ct);

    /// <summary>Get a single project by ID.</summary>
    [HttpGet("{id:int}")]
    public Task<IActionResult> Get(int id, CancellationToken ct)
        => ForwardGetAsync($"/api/projects/{id}", ct);

    /// <summary>Create a new project.</summary>
    [HttpPost]
    public Task<IActionResult> Create([FromBody] ProjectDto dto, CancellationToken ct)
        => ForwardPostAsync("/api/projects", dto, ct);

    /// <summary>Change only the status of a project (no other fields modified).</summary>
    [HttpPatch("{id:int}/status")]
    public Task<IActionResult> ChangeStatus(int id, [FromBody] ProjectStatusChangeRequest req, CancellationToken ct)
        => ForwardPatchAsync($"/api/projects/{id}/status", req, ct);
}
