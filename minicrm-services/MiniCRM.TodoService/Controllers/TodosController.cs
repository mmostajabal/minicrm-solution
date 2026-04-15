using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using MiniCRM.Shared.DTOs;
using MiniCRM.Shared.Exceptions;
using MiniCRM.Shared.Services;

namespace MiniCRM.TodoService.Controllers;

[ApiController]
[Route("api/todos")]
[Produces("application/json")]
public class TodosController : ControllerBase
{
    private const string CachePrefix = "todos:";
    private static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(60);

    private readonly ITodoApiClient _api;
    private readonly ICacheService _cache;
    private readonly ILogger<TodosController> _logger;

    public TodosController(ITodoApiClient api, ICacheService cache, ILogger<TodosController> logger)
    {
        _api    = api;
        _cache  = cache;
        _logger = logger;
    }

    /// <summary>Get todo list for a card (CardId = ProjectId in R3 API).</summary>
    [HttpGet("{cardId:int}")]
    public async Task<IActionResult> GetList(int cardId, CancellationToken ct)
    {
        var key = $"{CachePrefix}card:{cardId}";
        var cached = await _cache.GetAsync<TodoListResponse>(key, ct);
        if (cached is not null) return Ok(cached);

        try
        {
            var result = await _api.GetListAsync(cardId, ct);
            await _cache.SetAsync(key, result, CacheTtl, ct);
            return Ok(result);
        }
        catch (MiniCrmException ex) { return StatusCode(ex.StatusCode ?? 500, new { error = ex.Message }); }
    }

    /// <summary>Create a new todo (POST /Api/R3/ToDo).</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TodoCreateRequest req, CancellationToken ct)
    {
        try
        {
            var result = await _api.CreateAsync(req, ct);
            await _cache.RemoveByPrefixAsync($"{CachePrefix}card:{req.ProjectId}", ct);
            return Created($"/api/todos/{req.ProjectId}", result);
        }
        catch (MiniCrmException ex) { return StatusCode(ex.StatusCode ?? 500, new { error = ex.Message }); }
    }
}

// --- Interface ---

public interface ITodoApiClient
{
    Task<TodoListResponse> GetListAsync(int cardId, CancellationToken ct);
    Task<TodoDto> CreateAsync(TodoCreateRequest req, CancellationToken ct);
}

// --- miniCRM API client ---

public class TodoApiClient : MiniCrmClientBase, ITodoApiClient
{
    private static readonly JsonSerializerOptions Opts = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public TodoApiClient(HttpClient http, IConfiguration config, ILogger<TodoApiClient> logger)
        : base(http, config, logger) { }

    public async Task<TodoListResponse> GetListAsync(int cardId, CancellationToken ct)
    {
        await AcquireRateLimitAsync(ct);
        Logger.LogInformation("GET /Api/R3/Todo?CardId={CardId}", cardId);
        var response = await Http.GetAsync($"/Api/R3/Todo?CardId={cardId}", ct);
        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<TodoListResponse>(Opts, ct)
               ?? new TodoListResponse();
    }

    public async Task<TodoDto> CreateAsync(TodoCreateRequest req, CancellationToken ct)
    {
        await AcquireRateLimitAsync(ct);
        Logger.LogInformation("PUT /Api/R3/Todo (ProjectId={ProjectId})", req.ProjectId);
        var payload = JsonSerializer.Serialize(req, Opts);
        var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await Http.PutAsync("/Api/R3/Todo", content, ct);
        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<TodoDto>(Opts, ct)
               ?? throw new MiniCrmApiException("Empty response from ToDo create", 502);
    }
}
