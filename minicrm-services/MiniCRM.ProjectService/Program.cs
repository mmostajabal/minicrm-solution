using MiniCRM.ProjectService;
using MiniCRM.ProjectService.Controllers;
using MiniCRM.Shared.Services;
using Serilog;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new() { Title = "ProjectService", Version = "v1" }));

// Redis cache
var redisUrl = builder.Configuration["Redis:Url"] ?? "localhost:6379";
builder.Services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisUrl));
builder.Services.AddSingleton<ICacheService, RedisCacheService>();

builder.Services.AddHttpClient<ProjectApiClient>();
builder.Services.AddScoped<IProjectApiClient>(sp => sp.GetRequiredService<ProjectApiClient>());

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseSerilogRequestLogging();
app.MapControllers();

Log.Information("MiniCRM.ProjectService starting on port 5082");
app.Run("http://0.0.0.0:5082");
