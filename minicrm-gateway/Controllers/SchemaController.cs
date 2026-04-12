using Microsoft.AspNetCore.Mvc;

namespace MiniCRM.Gateway.Controllers;

/// <summary>
/// Gateway proxy for Schema/metadata → forwards to ContactService which aggregates schema calls.
/// </summary>
[ApiController]
[Route("api/schema")]
[Produces("application/json")]
public class SchemaController : GatewayControllerBase
{
    public SchemaController(IHttpClientFactory factory, IConfiguration config, ILogger<SchemaController> logger)
        : base(factory, "ContactService", config, logger) { }

    /// <summary>Get available categories and statuses.</summary>
    [HttpGet("categories")]
    public Task<IActionResult> GetCategories(CancellationToken ct)
        => ForwardGetAsync("/api/schema/categories", ct);

    /// <summary>Get custom field schema for a given type.</summary>
    [HttpGet("{type}")]
    public Task<IActionResult> GetSchema(string type, CancellationToken ct)
        => ForwardGetAsync($"/api/schema/{type}", ct);
}
