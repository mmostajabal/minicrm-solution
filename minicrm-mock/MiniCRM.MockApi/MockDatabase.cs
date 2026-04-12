using System.Text.Json;

/// <summary>
/// In-memory store that mimics the miniCRM REST API data layer.
/// Pre-seeded with realistic Hungarian CRM data.
/// All data resets when the mock server restarts.
/// </summary>
public class MockDatabase
{
    private int _nextContactId = 100;
    private int _nextProjectId = 200;
    private int _nextTodoId    = 300;
    private int _nextInvoiceId = 400;

    public Dictionary<int, ContactData>  Contacts { get; } = new();
    public Dictionary<int, ProjectData>  Projects { get; } = new();
    public Dictionary<int, TodoData>     Todos    { get; } = new();
    public Dictionary<int, InvoiceData>  Invoices { get; } = new();

    public List<CategoryData> ProjectCategories { get; } = new();
    public List<CategoryData> ContactCategories { get; } = new();

    public MockDatabase()
    {
        SeedCategories();
        SeedContacts();
        SeedProjects();
        SeedTodos();
        SeedInvoices();
    }

    // --- Factories ---

    public ContactData CreateContact(Dictionary<string, JsonElement>? body)
    {
        var id = _nextContactId++;
        var c  = new ContactData
        {
            Id          = id,
            FirstName   = GetString(body, "FirstName")   ?? "Új",
            LastName    = GetString(body, "LastName")    ?? "Kontakt",
            Email       = GetString(body, "Email"),
            Phone       = GetString(body, "Phone"),
            Type        = GetString(body, "Type")        ?? "Person",
            Description = GetString(body, "Description"),
            Position    = GetString(body, "Position"),
            StatusId    = GetInt(body, "StatusId") ?? 1,
            StatusName  = "Aktív",
            UserId      = 1,
            UserName    = "Teszt Felhasználó"
        };
        Contacts[id] = c;
        return c;
    }

    public ContactData UpdateContact(int id, Dictionary<string, JsonElement>? body)
    {
        var c = Contacts[id];
        if (body == null) return c;
        if (body.TryGetValue("FirstName",   out var fn)) c.FirstName   = fn.GetString();
        if (body.TryGetValue("LastName",    out var ln)) c.LastName    = ln.GetString();
        if (body.TryGetValue("Email",       out var em)) c.Email       = em.GetString();
        if (body.TryGetValue("Phone",       out var ph)) c.Phone       = ph.GetString();
        if (body.TryGetValue("Description", out var ds)) c.Description = ds.GetString();
        if (body.TryGetValue("Position",    out var ps)) c.Position    = ps.GetString();
        if (body.TryGetValue("StatusId",    out var si)) c.StatusId    = si.GetInt32();
        return c;
    }

    public ProjectData CreateProject(Dictionary<string, JsonElement>? body)
    {
        var id  = _nextProjectId++;
        var now = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
        var catId = GetInt(body, "CategoryId") ?? 1;
        var stId  = GetInt(body, "StatusId")   ?? ProjectCategories.FirstOrDefault(c => c.Id == catId)?.Statuses?.FirstOrDefault()?.Id ?? 1;
        var contactId = GetInt(body, "ContactId");
        var p = new ProjectData
        {
            Id           = id,
            Name         = GetString(body, "Name") ?? "Új Projekt",
            CategoryId   = catId,
            CategoryName = ProjectCategories.FirstOrDefault(c => c.Id == catId)?.Name ?? "Általános",
            StatusId     = stId,
            StatusName   = GetProjectStatusName(stId),
            ContactId    = contactId,
            ContactName  = contactId.HasValue && Contacts.TryGetValue(contactId.Value, out var ct2)
                           ? $"{ct2.LastName} {ct2.FirstName}" : null,
            UserId       = 1,
            UserName     = "Teszt Felhasználó",
            Description  = GetString(body, "Description"),
            CreatedAt    = now,
            UpdatedAt    = now
        };
        Projects[id] = p;
        return p;
    }

    public TodoData CreateTodo(Dictionary<string, JsonElement>? body)
    {
        var id  = _nextTodoId++;
        var now = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
        var t = new TodoData
        {
            Id        = id,
            ProjectId = GetInt(body, "ProjectId") ?? GetInt(body, "CardId") ?? 0,
            UserId    = GetInt(body, "UserId") ?? 1,
            UserName  = "Teszt Felhasználó",
            Type      = GetString(body, "Type") ?? "Call",
            Deadline  = GetString(body, "Deadline") ?? DateTime.UtcNow.AddDays(3).ToString("yyyy-MM-dd HH:mm:ss"),
            Comment   = GetString(body, "Comment"),
            Status    = 0,
            CreatedAt = now
        };
        Todos[id] = t;
        return t;
    }

    public string GetProjectStatusName(int statusId)
    {
        foreach (var cat in ProjectCategories)
            foreach (var st in cat.Statuses ?? new())
                if (st.Id == statusId) return st.Name ?? statusId.ToString();
        return statusId.ToString();
    }

    // --- Seed data ---

    private void SeedCategories()
    {
        ProjectCategories.AddRange(new[]
        {
            new CategoryData { Id = 1, Name = "Értékesítés", Type = "Project", Statuses = new()
            {
                new StatusData { Id = 10, Name = "Érdeklődő",    Color = "#3498db", IsOpen = true  },
                new StatusData { Id = 11, Name = "Ajánlat küldve", Color = "#f39c12", IsOpen = true  },
                new StatusData { Id = 12, Name = "Tárgyalás",    Color = "#e67e22", IsOpen = true  },
                new StatusData { Id = 13, Name = "Nyertes",      Color = "#27ae60", IsOpen = false },
                new StatusData { Id = 14, Name = "Elveszített",  Color = "#e74c3c", IsOpen = false },
            }},
            new CategoryData { Id = 2, Name = "Támogatás", Type = "Project", Statuses = new()
            {
                new StatusData { Id = 20, Name = "Új ticket",    Color = "#3498db", IsOpen = true  },
                new StatusData { Id = 21, Name = "Folyamatban",  Color = "#f39c12", IsOpen = true  },
                new StatusData { Id = 22, Name = "Megoldva",     Color = "#27ae60", IsOpen = false },
            }},
            new CategoryData { Id = 3, Name = "Projekt", Type = "Project", Statuses = new()
            {
                new StatusData { Id = 30, Name = "Tervezés",     Color = "#9b59b6", IsOpen = true  },
                new StatusData { Id = 31, Name = "Fejlesztés",   Color = "#3498db", IsOpen = true  },
                new StatusData { Id = 32, Name = "Tesztelés",    Color = "#f39c12", IsOpen = true  },
                new StatusData { Id = 33, Name = "Befejezett",   Color = "#27ae60", IsOpen = false },
            }}
        });

        ContactCategories.AddRange(new[]
        {
            new CategoryData { Id = 4, Name = "Ügyfél", Type = "Contact", Statuses = new()
            {
                new StatusData { Id = 40, Name = "Aktív",        Color = "#27ae60", IsOpen = true  },
                new StatusData { Id = 41, Name = "Inaktív",      Color = "#95a5a6", IsOpen = false },
            }},
            new CategoryData { Id = 5, Name = "Partner", Type = "Contact", Statuses = new()
            {
                new StatusData { Id = 50, Name = "Aktív",        Color = "#27ae60", IsOpen = true  },
                new StatusData { Id = 51, Name = "Archivált",    Color = "#95a5a6", IsOpen = false },
            }}
        });
    }

    private void SeedContacts()
    {
        var contacts = new[]
        {
            new ContactData { Id = 1, FirstName = "Gábor",   LastName = "Nagy",    Email = "nagy.gabor@example.hu",    Phone = "+36 20 123 4567", Type = "Person",  StatusId = 40, StatusName = "Aktív",   UserId = 1, UserName = "Teszt Felhasználó" },
            new ContactData { Id = 2, FirstName = "Eszter",  LastName = "Kovács",  Email = "kovacs.eszter@example.hu", Phone = "+36 30 234 5678", Type = "Person",  StatusId = 40, StatusName = "Aktív",   UserId = 1, UserName = "Teszt Felhasználó" },
            new ContactData { Id = 3, FirstName = "Péter",   LastName = "Tóth",    Email = "toth.peter@cegnev.hu",     Phone = "+36 70 345 6789", Type = "Company", StatusId = 40, StatusName = "Aktív",   UserId = 1, UserName = "Teszt Felhasználó" },
            new ContactData { Id = 4, FirstName = "Katalin", LastName = "Szabó",   Email = "szabo.kata@example.hu",    Phone = "+36 20 456 7890", Type = "Person",  StatusId = 41, StatusName = "Inaktív", UserId = 1, UserName = "Teszt Felhasználó" },
            new ContactData { Id = 5, FirstName = "Dániel",  LastName = "Horváth", Email = "horvath.d@partnerceg.hu",  Phone = "+36 30 567 8901", Type = "Company", StatusId = 50, StatusName = "Aktív",   UserId = 1, UserName = "Teszt Felhasználó" },
        };
        foreach (var c in contacts) Contacts[c.Id!.Value] = c;
        _nextContactId = 100;
    }

    private void SeedProjects()
    {
        var now = DateTime.UtcNow;
        var projects = new[]
        {
            new ProjectData { Id = 1, Name = "Webshop fejlesztés",      CategoryId = 3, CategoryName = "Projekt",     StatusId = 31, StatusName = "Fejlesztés",    ContactId = 3, ContactName = "Tóth Péter",    UserId = 1, UserName = "Teszt Felhasználó", Description = "E-commerce platform fejlesztése",  CreatedAt = now.AddDays(-30).ToString("yyyy-MM-dd"), UpdatedAt = now.AddDays(-2).ToString("yyyy-MM-dd") },
            new ProjectData { Id = 2, Name = "ERP integráció ajánlat",  CategoryId = 1, CategoryName = "Értékesítés", StatusId = 11, StatusName = "Ajánlat küldve", ContactId = 5, ContactName = "Horváth Dániel", UserId = 1, UserName = "Teszt Felhasználó", Description = "SAP integráció",                   CreatedAt = now.AddDays(-15).ToString("yyyy-MM-dd"), UpdatedAt = now.AddDays(-1).ToString("yyyy-MM-dd") },
            new ProjectData { Id = 3, Name = "Support ticket – login",  CategoryId = 2, CategoryName = "Támogatás",   StatusId = 21, StatusName = "Folyamatban",    ContactId = 1, ContactName = "Nagy Gábor",     UserId = 1, UserName = "Teszt Felhasználó", Description = "Bejelentkezési hiba",               CreatedAt = now.AddDays(-5).ToString("yyyy-MM-dd"),  UpdatedAt = now.AddDays(-1).ToString("yyyy-MM-dd") },
            new ProjectData { Id = 4, Name = "Marketing kampány Q2",    CategoryId = 1, CategoryName = "Értékesítés", StatusId = 10, StatusName = "Érdeklődő",      ContactId = 2, ContactName = "Kovács Eszter",  UserId = 1, UserName = "Teszt Felhasználó", Description = "Q2 email kampány",                  CreatedAt = now.AddDays(-3).ToString("yyyy-MM-dd"),  UpdatedAt = now.ToString("yyyy-MM-dd") },
        };
        foreach (var p in projects) Projects[p.Id!.Value] = p;
        _nextProjectId = 200;
    }

    private void SeedTodos()
    {
        var todos = new[]
        {
            new TodoData { Id = 1, ProjectId = 1, UserId = 1, UserName = "Teszt Felhasználó", Type = "Meeting",  Deadline = DateTime.UtcNow.AddDays(2).ToString("yyyy-MM-dd HH:mm:ss"),  Comment = "Kick-off megbeszélés",    Status = 0, CreatedAt = DateTime.UtcNow.AddDays(-5).ToString("yyyy-MM-dd") },
            new TodoData { Id = 2, ProjectId = 2, UserId = 1, UserName = "Teszt Felhasználó", Type = "Call",     Deadline = DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss"),  Comment = "Ajánlat egyeztetés",      Status = 0, CreatedAt = DateTime.UtcNow.AddDays(-3).ToString("yyyy-MM-dd") },
            new TodoData { Id = 3, ProjectId = 3, UserId = 1, UserName = "Teszt Felhasználó", Type = "Email",    Deadline = DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"), Comment = "Hibajelentés visszajelzés", Status = 1, CreatedAt = DateTime.UtcNow.AddDays(-5).ToString("yyyy-MM-dd") },
        };
        foreach (var t in todos) Todos[t.Id!.Value] = t;
        _nextTodoId = 300;
    }

    private void SeedInvoices()
    {
        var invoices = new[]
        {
            new InvoiceData { Id = 1, InvoiceNumber = "INV-2026-001", ProjectId = 1, ContactId = 3, ContactName = "Tóth Péter",    Amount = 850000,  Currency = "HUF", Status = "Unpaid", IssueDate = "2026-03-01", DueDate = "2026-03-31" },
            new InvoiceData { Id = 2, InvoiceNumber = "INV-2026-002", ProjectId = 2, ContactId = 5, ContactName = "Horváth Dániel", Amount = 1250000, Currency = "HUF", Status = "Paid",   IssueDate = "2026-02-15", DueDate = "2026-03-15", PaidDate = "2026-03-10" },
        };
        foreach (var i in invoices) Invoices[i.Id!.Value] = i;
        _nextInvoiceId = 400;
    }

    // --- Helpers ---

    private static string? GetString(Dictionary<string, JsonElement>? body, string key)
        => body != null && body.TryGetValue(key, out var el) && el.ValueKind == JsonValueKind.String
            ? el.GetString() : null;

    private static int? GetInt(Dictionary<string, JsonElement>? body, string key)
        => body != null && body.TryGetValue(key, out var el) && el.ValueKind == JsonValueKind.Number
            ? el.GetInt32() : null;
}

// --- Data models ---

public class ContactData
{
    public int?    Id         { get; set; }
    public string? FirstName  { get; set; }
    public string? LastName   { get; set; }
    public string? Email      { get; set; }
    public string? Phone      { get; set; }
    public string? Type       { get; set; }
    public string? Description{ get; set; }
    public string? Position   { get; set; }
    public int?    StatusId   { get; set; }
    public string? StatusName { get; set; }
    public int?    UserId     { get; set; }
    public string? UserName   { get; set; }
}

public class ProjectData
{
    public int?    Id           { get; set; }
    public string? Name         { get; set; }
    public int?    CategoryId   { get; set; }
    public string? CategoryName { get; set; }
    public int?    StatusId     { get; set; }
    public string? StatusName   { get; set; }
    public int?    ContactId    { get; set; }
    public string? ContactName  { get; set; }
    public int?    UserId       { get; set; }
    public string? UserName     { get; set; }
    public string? Description  { get; set; }
    public string? CreatedAt    { get; set; }
    public string? UpdatedAt    { get; set; }
}

public class TodoData
{
    public int?    Id        { get; set; }
    public int?    ProjectId { get; set; }
    public int?    UserId    { get; set; }
    public string? UserName  { get; set; }
    public string? Type      { get; set; }
    public string? Deadline  { get; set; }
    public string? Comment   { get; set; }
    public int?    Status    { get; set; }
    public string? CreatedAt { get; set; }
}

public class InvoiceData
{
    public int?     Id            { get; set; }
    public string?  InvoiceNumber { get; set; }
    public int?     ProjectId     { get; set; }
    public int?     ContactId     { get; set; }
    public string?  ContactName   { get; set; }
    public decimal? Amount        { get; set; }
    public string?  Currency      { get; set; }
    public string?  Status        { get; set; }
    public string?  DueDate       { get; set; }
    public string?  IssueDate     { get; set; }
    public string?  PaidDate      { get; set; }
}

public class CategoryData
{
    public int              Id       { get; set; }
    public string?          Name     { get; set; }
    public string?          Type     { get; set; }
    public List<StatusData>? Statuses { get; set; }
}

public class StatusData
{
    public int     Id     { get; set; }
    public string? Name   { get; set; }
    public string? Color  { get; set; }
    public bool    IsOpen { get; set; }
}
