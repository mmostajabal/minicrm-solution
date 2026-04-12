using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<MockDatabase>();

var app = builder.Build();

var jsonOptions = new JsonSerializerOptions
{
    PropertyNamingPolicy   = null,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    WriteIndented          = true
};

// --- Contacts ---

app.MapGet("/Api/R3/Contact", (
    MockDatabase db,
    string? Name, string? Email, string? Phone,
    int? StatusId, int? UserId,
    int Page = 1, int MaxElements = 100) =>
{
    var results = db.Contacts.Values
        .Where(c =>
            (Name     == null || (c.LastName + " " + c.FirstName).Contains(Name,  StringComparison.OrdinalIgnoreCase)) &&
            (Email    == null || (c.Email  ?? "").Contains(Email,  StringComparison.OrdinalIgnoreCase)) &&
            (Phone    == null || (c.Phone  ?? "").Contains(Phone,  StringComparison.OrdinalIgnoreCase)) &&
            (StatusId == null || c.StatusId == StatusId) &&
            (UserId   == null || c.UserId   == UserId))
        .Skip((Page - 1) * MaxElements).Take(MaxElements).ToList();

    var dict = results.ToDictionary(c => c.Id!.Value.ToString(), c => c);
    return Results.Json(new { Count = dict.Count, CurrentPage = Page, Results = dict }, jsonOptions);
});

app.MapGet("/Api/R3/Contact/{id:int}", (MockDatabase db, int id) =>
{
    if (!db.Contacts.TryGetValue(id, out var c))
        return Results.Json(new { Code = 404, Message = "Contact not found" }, jsonOptions, statusCode: 404);
    return Results.Json(c, jsonOptions);
});

app.MapPut("/Api/R3/Contact", async (MockDatabase db, HttpRequest req) =>
{
    var body = await JsonSerializer.DeserializeAsync<Dictionary<string, JsonElement>>(req.Body);
    return Results.Json(db.CreateContact(body), jsonOptions, statusCode: 201);
});

app.MapPut("/Api/R3/Contact/{id:int}", async (MockDatabase db, int id, HttpRequest req) =>
{
    if (!db.Contacts.ContainsKey(id))
        return Results.Json(new { Code = 404, Message = "Contact not found" }, jsonOptions, statusCode: 404);
    var body = await JsonSerializer.DeserializeAsync<Dictionary<string, JsonElement>>(req.Body);
    return Results.Json(db.UpdateContact(id, body), jsonOptions);
});

// --- Projects ---

app.MapGet("/Api/R3/Project", (
    MockDatabase db,
    int? CategoryId, int? StatusId, int? ContactId, int? UserId, string? Name,
    int Page = 1, int MaxElements = 100) =>
{
    var results = db.Projects.Values
        .Where(p =>
            (Name       == null || (p.Name ?? "").Contains(Name, StringComparison.OrdinalIgnoreCase)) &&
            (CategoryId == null || p.CategoryId == CategoryId) &&
            (StatusId   == null || p.StatusId   == StatusId) &&
            (ContactId  == null || p.ContactId  == ContactId) &&
            (UserId     == null || p.UserId     == UserId))
        .Skip((Page - 1) * MaxElements).Take(MaxElements).ToList();

    var dict = results.ToDictionary(p => p.Id!.Value.ToString(), p => p);
    return Results.Json(new { Count = dict.Count, CurrentPage = Page, Results = dict }, jsonOptions);
});

app.MapGet("/Api/R3/Project/{id:int}", (MockDatabase db, int id) =>
{
    if (!db.Projects.TryGetValue(id, out var p))
        return Results.Json(new { Code = 404, Message = "Project not found" }, jsonOptions, statusCode: 404);
    return Results.Json(p, jsonOptions);
});

app.MapPut("/Api/R3/Project", async (MockDatabase db, HttpRequest req) =>
{
    var body = await JsonSerializer.DeserializeAsync<Dictionary<string, JsonElement>>(req.Body);
    return Results.Json(db.CreateProject(body), jsonOptions, statusCode: 201);
});

app.MapPut("/Api/R3/Project/{id:int}", async (MockDatabase db, int id, HttpRequest req) =>
{
    if (!db.Projects.TryGetValue(id, out var project))
        return Results.Json(new { Code = 404, Message = "Project not found" }, jsonOptions, statusCode: 404);
    var body = await JsonSerializer.DeserializeAsync<Dictionary<string, JsonElement>>(req.Body);
    if (body != null && body.TryGetValue("StatusId", out var el))
    {
        project.StatusId   = el.GetInt32();
        project.StatusName = db.GetProjectStatusName(project.StatusId.Value);
    }
    project.UpdatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
    return Results.Json(project, jsonOptions);
});

// --- Todos ---

app.MapGet("/Api/R3/Todo", (MockDatabase db, int? CardId) =>
{
    var results = db.Todos.Values
        .Where(t => CardId == null || t.ProjectId == CardId).ToList();
    return Results.Json(new { Count = results.Count, Results = results }, jsonOptions);
});

app.MapPut("/Api/R3/Todo", async (MockDatabase db, HttpRequest req) =>
{
    var body = await JsonSerializer.DeserializeAsync<Dictionary<string, JsonElement>>(req.Body);
    return Results.Json(db.CreateTodo(body), jsonOptions, statusCode: 201);
});

// --- Categories ---

app.MapGet("/Api/R3/Category", (MockDatabase db, string? CategoryType) =>
{
    var categories = CategoryType?.ToLower() switch
    {
        "contact" => db.ContactCategories,
        _         => db.ProjectCategories
    };
    return Results.Json(new { Results = categories }, jsonOptions);
});

// --- Invoices ---

app.MapGet("/Api/Invoice", (MockDatabase db, int? ProjectId, int? ContactId, string? Status) =>
{
    var results = db.Invoices.Values
        .Where(i =>
            (ProjectId == null || i.ProjectId == ProjectId) &&
            (ContactId == null || i.ContactId == ContactId) &&
            (Status    == null || i.Status    == Status)).ToList();
    return Results.Json(new { Count = results.Count, CurrentPage = 1, Results = results }, jsonOptions);
});

app.MapGet("/Api/Invoice/{id:int}", (MockDatabase db, int id) =>
{
    if (!db.Invoices.TryGetValue(id, out var inv))
        return Results.Json(new { Code = 404, Message = "Invoice not found" }, jsonOptions, statusCode: 404);
    return Results.Json(inv, jsonOptions);
});

Console.WriteLine("╔══════════════════════════════════════════╗");
Console.WriteLine("║   MiniCRM Mock API  →  port 5090         ║");
Console.WriteLine("╚══════════════════════════════════════════╝");
app.Run("http://0.0.0.0:5090");
