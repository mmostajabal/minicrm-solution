using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using MiniCRM.Shared.DTOs;
using MiniCRM.Shared.Exceptions;

namespace MiniCRM.ContactService.Controllers;

[ApiController]
[Route("api/contacts")]
[Produces("application/json")]
public class ContactsController : ControllerBase
{
    private readonly IContactApiClient _api;
    private readonly ILogger<ContactsController> _logger;

    public ContactsController(IContactApiClient api, ILogger<ContactsController> logger)
    {
        _api    = api;
        _logger = logger;
    }

    /// <summary>Search contacts by name, email or phone.</summary>
    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] ContactSearchParams p, CancellationToken ct)
    {
        try
        {
            var result = await _api.SearchAsync(p, ct);
            return Ok(result);
        }
        catch (MiniCrmException ex)
        {
            _logger.LogWarning(ex, "Contact search failed");
            return StatusCode(ex.StatusCode ?? 500, new { error = ex.Message });
        }
    }

    /// <summary>Get a contact by ID.</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken ct)
    {
        try
        {
            var result = await _api.GetByIdAsync(id, ct);
            return Ok(result);
        }
        catch (MiniCrmNotFoundException)
        {
            return NotFound(new { error = $"Contact {id} not found." });
        }
        catch (MiniCrmException ex)
        {
            _logger.LogWarning(ex, "Contact GET {Id} failed", id);
            return StatusCode(ex.StatusCode ?? 500, new { error = ex.Message });
        }
    }

    /// <summary>Create a new contact (PUT /Api/R3/Contact).</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ContactDto dto, CancellationToken ct)
    {
        try
        {
            var result = await _api.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }
        catch (MiniCrmException ex)
        {
            _logger.LogWarning(ex, "Contact create failed");
            return StatusCode(ex.StatusCode ?? 500, new { error = ex.Message });
        }
    }

    /// <summary>Update a contact (PUT /Api/R3/Contact/{Id}).</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ContactDto dto, CancellationToken ct)
    {
        dto.Id = id;
        try
        {
            var result = await _api.UpdateAsync(id, dto, ct);
            return Ok(result);
        }
        catch (MiniCrmNotFoundException)
        {
            return NotFound(new { error = $"Contact {id} not found." });
        }
        catch (MiniCrmException ex)
        {
            _logger.LogWarning(ex, "Contact update {Id} failed", id);
            return StatusCode(ex.StatusCode ?? 500, new { error = ex.Message });
        }
    }
}

// --- Interface ---

public interface IContactApiClient
{
    Task<ContactListResponse> SearchAsync(ContactSearchParams p, CancellationToken ct);
    Task<ContactDto> GetByIdAsync(int id, CancellationToken ct);
    Task<ContactDto> CreateAsync(ContactDto dto, CancellationToken ct);
    Task<ContactDto> UpdateAsync(int id, ContactDto dto, CancellationToken ct);
}

// --- miniCRM API client ---

public class ContactApiClient : MiniCrmClientBase, IContactApiClient
{
    private static readonly JsonSerializerOptions Opts = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public ContactApiClient(HttpClient http, IConfiguration config, ILogger<ContactApiClient> logger)
        : base(http, config, logger) { }

    public async Task<ContactListResponse> SearchAsync(ContactSearchParams p, CancellationToken ct)
    {
        await AcquireRateLimitAsync(ct);
        var qs = BuildContactQuery(p);
        Logger.LogInformation("GET /Api/R3/Contact?{Query}", qs);
        var response = await Http.GetAsync($"/Api/R3/Contact?{qs}", ct);
        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<ContactListResponse>(Opts, ct)
               ?? new ContactListResponse();
    }

    public async Task<ContactDto> GetByIdAsync(int id, CancellationToken ct)
    {
        await AcquireRateLimitAsync(ct);
        Logger.LogInformation("GET /Api/R3/Contact/{Id}", id);
        var response = await Http.GetAsync($"/Api/R3/Contact/{id}", ct);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            throw new MiniCrmNotFoundException("Contact", id);
        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<ContactDto>(Opts, ct)
               ?? throw new MiniCrmApiException("Empty response from Contact GET", 502);
    }

    public async Task<ContactDto> CreateAsync(ContactDto dto, CancellationToken ct)
    {
        await AcquireRateLimitAsync(ct);
        Logger.LogInformation("PUT /Api/R3/Contact (create)");
        // miniCRM uses PUT without Id to create
        var payload = JsonSerializer.Serialize(dto, Opts);
        var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await Http.PutAsync("/Api/R3/Contact", content, ct);
        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<ContactDto>(Opts, ct)
               ?? throw new MiniCrmApiException("Empty response from Contact create", 502);
    }

    public async Task<ContactDto> UpdateAsync(int id, ContactDto dto, CancellationToken ct)
    {
        await AcquireRateLimitAsync(ct);
        Logger.LogInformation("PUT /Api/R3/Contact/{Id} (update)", id);
        var payload = JsonSerializer.Serialize(dto, Opts);
        var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await Http.PutAsync($"/Api/R3/Contact/{id}", content, ct);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            throw new MiniCrmNotFoundException("Contact", id);
        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<ContactDto>(Opts, ct)
               ?? throw new MiniCrmApiException("Empty response from Contact update", 502);
    }

    private static string BuildContactQuery(ContactSearchParams p)
    {
        var q = HttpUtility.ParseQueryString(string.Empty);
        if (!string.IsNullOrWhiteSpace(p.Name))  q["Name"]  = p.Name;
        if (!string.IsNullOrWhiteSpace(p.Email)) q["Email"] = p.Email;
        if (!string.IsNullOrWhiteSpace(p.Phone)) q["Phone"] = p.Phone;
        if (p.StatusId.HasValue) q["StatusId"] = p.StatusId.ToString();
        if (p.UserId.HasValue)   q["UserId"]   = p.UserId.ToString();
        q["Page"]        = p.Page.ToString();
        q["MaxElements"] = p.MaxElements.ToString();
        return q.ToString()!;
    }
}
