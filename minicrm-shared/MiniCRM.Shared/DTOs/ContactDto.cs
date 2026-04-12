namespace MiniCRM.Shared.DTOs;

/// <summary>
/// Contact data transfer object matching miniCRM /Api/R3/Contact schema.
/// </summary>
public class ContactDto
{
    public int? Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Type { get; set; }          // "Person" | "Company"
    public string? Description { get; set; }
    public string? Position { get; set; }
    public int? StatusId { get; set; }
    public string? StatusName { get; set; }
    public int? UserId { get; set; }
    public string? UserName { get; set; }
    public Dictionary<string, object?>? Fields { get; set; }
}

public class ContactSearchParams
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public int? StatusId { get; set; }
    public int? UserId { get; set; }
    public int Page { get; set; } = 1;
    public int MaxElements { get; set; } = 100;
}

public class ContactListResponse
{
    public int Count { get; set; }
    public int CurrentPage { get; set; }
    public Dictionary<string, ContactDto>? Results { get; set; }
}
