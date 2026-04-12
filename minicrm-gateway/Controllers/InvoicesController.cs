using Microsoft.AspNetCore.Mvc;
using MiniCRM.Shared.DTOs;

namespace MiniCRM.Gateway.Controllers;

/// <summary>
/// Gateway proxy for Invoice operations → forwards to InvoiceService (port 5004).
/// Read-only – no write endpoints per spec.
/// </summary>
[ApiController]
[Route("api/invoices")]
[Produces("application/json")]
public class InvoicesController : GatewayControllerBase
{
    public InvoicesController(IHttpClientFactory factory, IConfiguration config, ILogger<InvoicesController> logger)
        : base(factory, "InvoiceService", config, logger) { }

    /// <summary>Query invoices by project or contact.</summary>
    [HttpGet]
    public Task<IActionResult> Query([FromQuery] InvoiceSearchParams p, CancellationToken ct)
        => ForwardGetAsync($"/api/invoices?{QueryFrom(p)}", ct);
}
