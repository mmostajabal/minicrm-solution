using MiniCRM.TodoService;
using MiniCRM.TodoService.Controllers;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new() { Title = "TodoService", Version = "v1" }));

builder.Services.AddHttpClient<TodoApiClient>();
builder.Services.AddScoped<ITodoApiClient>(sp => sp.GetRequiredService<TodoApiClient>());

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseSerilogRequestLogging();
app.MapControllers();

Log.Information("MiniCRM.TodoService starting on port 5083");
app.Run("http://0.0.0.0:5083");
