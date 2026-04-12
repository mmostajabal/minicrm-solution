namespace MiniCRM.Shared.DTOs;

/// <summary>
/// ToDo DTO matching miniCRM /Api/R3/ToDo schema.
/// </summary>
public class TodoDto
{
    public int? Id { get; set; }
    public int? ProjectId { get; set; }
    public int? UserId { get; set; }
    public string? UserName { get; set; }
    public string? Type { get; set; }           // e.g. "Call", "Meeting", "Email"
    public string? Deadline { get; set; }        // ISO 8601 datetime string
    public string? Comment { get; set; }
    public int? Status { get; set; }             // 0 = open, 1 = done
    public string? CreatedAt { get; set; }
}

public class TodoCreateRequest
{
    public int ProjectId { get; set; }
    public int UserId { get; set; }
    public string Type { get; set; } = "Call";
    public string? Deadline { get; set; }
    public string? Comment { get; set; }
}

public class TodoListResponse
{
    public int Count { get; set; }
    public List<TodoDto>? Results { get; set; }
}
