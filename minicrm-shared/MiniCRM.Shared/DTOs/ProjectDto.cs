namespace MiniCRM.Shared.DTOs;

/// <summary>
/// Project (deal) DTO matching miniCRM /Api/R3/Project schema.
/// </summary>
public class ProjectDto
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public int? StatusId { get; set; }
    public string? StatusName { get; set; }
    public int? ContactId { get; set; }
    public string? ContactName { get; set; }
    public int? UserId { get; set; }
    public string? UserName { get; set; }
    public string? Description { get; set; }
    public string? CreatedAt { get; set; }
    public string? UpdatedAt { get; set; }
    public Dictionary<string, object?>? Fields { get; set; }
}

public class ProjectSearchParams
{
    public int? CategoryId { get; set; }
    public int? StatusId { get; set; }
    public int? ContactId { get; set; }
    public int? UserId { get; set; }
    public string? Name { get; set; }
    public int Page { get; set; } = 1;
    public int MaxElements { get; set; } = 100;
}

public class ProjectListResponse
{
    public int Count { get; set; }
    public int CurrentPage { get; set; }
    public Dictionary<string, ProjectDto>? Results { get; set; }
}

public class ProjectStatusChangeRequest
{
    public int StatusId { get; set; }
}
