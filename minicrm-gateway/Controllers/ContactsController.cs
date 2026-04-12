using Microsoft.AspNetCore.Mvc;
using MiniCRM.Shared.DTOs;

namespace MiniCRM.Gateway.Controllers;

/// <summary>
/// Gateway proxy for Contact operations → forwards to ContactService (port 5001).
/// </summary>
[ApiController]
[Route("api/contacts")]
[Produces("application/json")]
public class ContactsController : GatewayControllerBase
{
    public ContactsController(IHttpClientFactory factory, IConfiguration config, ILogger<ContactsController> logger)
        : base(factory, "ContactService", config, logger) { }

    /// <summary>Search contacts by name, email or phone.</summary>
    [HttpGet]
    public Task<IActionResult> Search([FromQuery] ContactSearchParams p, CancellationToken ct)
        => ForwardGetAsync($"/api/contacts?{QueryFrom(p)}", ct);

    /// <summary>Get a single contact by ID.</summary>
    [HttpGet("{id:int}")]
    public Task<IActionResult> Get(int id, CancellationToken ct)
        => ForwardGetAsync($"/api/contacts/{id}", ct);

    /// <summary>Create a new contact.</summary>
    [HttpPost]
    public Task<IActionResult> Create([FromBody] ContactDto dto, CancellationToken ct)
        => ForwardPostAsync("/api/contacts", dto, ct);

    /// <summary>Update an existing contact.</summary>
    [HttpPut("{id:int}")]
    public Task<IActionResult> Update(int id, [FromBody] ContactDto dto, CancellationToken ct)
        => ForwardPutAsync($"/api/contacts/{id}", dto, ct);
}
