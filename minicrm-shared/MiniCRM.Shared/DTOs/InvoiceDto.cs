namespace MiniCRM.Shared.DTOs;

/// <summary>
/// Invoice DTO matching miniCRM /Api/Invoice schema.
/// </summary>
public class InvoiceDto
{
    public int? Id { get; set; }
    public string? InvoiceNumber { get; set; }
    public int? ProjectId { get; set; }
    public int? ContactId { get; set; }
    public string? ContactName { get; set; }
    public decimal? Amount { get; set; }
    public string? Currency { get; set; }
    public string? Status { get; set; }          // "Paid", "Unpaid", "Overdue"
    public string? DueDate { get; set; }
    public string? IssueDate { get; set; }
    public string? PaidDate { get; set; }
}

public class InvoiceSearchParams
{
    public int? ProjectId { get; set; }
    public int? ContactId { get; set; }
    public string? Status { get; set; }
    public int Page { get; set; } = 1;
    public int MaxElements { get; set; } = 100;
}

public class InvoiceListResponse
{
    public int Count { get; set; }
    public int CurrentPage { get; set; }
    public List<InvoiceDto>? Results { get; set; }
}
