using Microsoft.AspNetCore.Mvc;
using MiniCRM.Shared.DTOs;

namespace MiniCRM.Gateway.Controllers;

/// <summary>
/// Gateway proxy for ToDo operations → forwards to TodoService (port 5003).
/// </summary>
[ApiController]
[Route("api/todos")]
[Produces("application/json")]
public class TodosController : GatewayControllerBase
{
    public TodosController(IHttpClientFactory factory, IConfiguration config, ILogger<TodosController> logger)
        : base(factory, "TodoService", config, logger) { }

    /// <summary>Get todo list for a project/card (CardId = ProjectId in R3).</summary>
    [HttpGet("{cardId:int}")]
    public Task<IActionResult> GetList(int cardId, CancellationToken ct)
        => ForwardGetAsync($"/api/todos/{cardId}", ct);

    /// <summary>Create a new todo.</summary>
    [HttpPost]
    public Task<IActionResult> Create([FromBody] TodoCreateRequest req, CancellationToken ct)
        => ForwardPostAsync("/api/todos", req, ct);
}
