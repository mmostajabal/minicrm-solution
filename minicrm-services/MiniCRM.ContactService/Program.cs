using MiniCRM.ContactService;
using MiniCRM.ContactService.Controllers;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new() { Title = "ContactService", Version = "v1" }));

// Register dedicated HttpClient + API clients
builder.Services.AddHttpClient<ContactApiClient>();
builder.Services.AddScoped<IContactApiClient>(sp => sp.GetRequiredService<ContactApiClient>());
builder.Services.AddHttpClient<SchemaApiClient>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseSerilogRequestLogging();
app.MapControllers();

Log.Information("MiniCRM.ContactService starting on port 5081");
app.Run("http://0.0.0.0:5081");
