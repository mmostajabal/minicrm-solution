namespace MiniCRM.Shared.DTOs;

/// <summary>
/// Schema / metadata DTO for miniCRM categories, statuses and custom fields.
/// </summary>
public class CategoryDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Type { get; set; }
    public List<StatusDto>? Statuses { get; set; }
}

public class StatusDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Color { get; set; }
    public bool IsOpen { get; set; }
}

public class SchemaFieldDto
{
    public string? Key { get; set; }
    public string? Label { get; set; }
    public string? Type { get; set; }   // "Text", "Number", "List", "Date", etc.
    public bool Required { get; set; }
    public List<string>? Options { get; set; }
}

public class SchemaResponse
{
    public List<CategoryDto>? Categories { get; set; }
    public List<SchemaFieldDto>? Fields { get; set; }
}
