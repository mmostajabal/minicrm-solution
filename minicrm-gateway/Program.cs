using System.Threading.RateLimiting;
using MiniCRM.Gateway.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// --- Serilog ---
builder.Host.UseSerilog((ctx, lc) =>
    lc.ReadFrom.Configuration(ctx.Configuration));

// --- Controllers + Swagger ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "miniCRM API Gateway",
        Version = "v1",
        Description = "Reverse-proxy gateway for miniCRM microservices. " +
                      "Authenticate with header X-Gateway-Key."
    });
    c.AddSecurityDefinition("GatewayKey", new()
    {
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Name = "X-Gateway-Key",
        Description = "Gateway API key set in appsettings.json → Gateway:GatewayApiKey"
    });
    c.AddSecurityRequirement(new()
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new() { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "GatewayKey" }
            },
            Array.Empty<string>()
        }
    });
});

// --- Named HttpClients for each downstream service ---
var services = builder.Configuration.GetSection("Services");
foreach (var svc in new[] { "ContactService", "ProjectService", "TodoService", "InvoiceService" })
{
    builder.Services.AddHttpClient(svc, client =>
    {
        client.BaseAddress = new Uri(services[svc]
            ?? throw new InvalidOperationException($"Services:{svc} missing from config."));
        client.Timeout = TimeSpan.FromSeconds(30);
    });
}

// --- Rate Limiting: 60 req/min sliding window ---
var rateLimit = builder.Configuration.GetValue<int>("Gateway:RateLimitPerMinute", 60);
builder.Services.AddRateLimiter(opt =>
{
    opt.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
        RateLimitPartition.GetSlidingWindowLimiter(
            "global",
            _ => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = rateLimit,
                Window = TimeSpan.FromMinutes(1),
                SegmentsPerWindow = 6,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));

    opt.OnRejected = async (ctx, ct) =>
    {
        ctx.HttpContext.Response.StatusCode = 429;
        await ctx.HttpContext.Response.WriteAsJsonAsync(
            new { error = "Rate limit exceeded. Maximum 60 requests per minute." }, ct);
    };
});

// --- Build ---
var app = builder.Build();

app.UseSerilogRequestLogging(opts =>
{
    opts.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
});

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "miniCRM Gateway v1");
    c.RoutePrefix = "swagger";
});

app.UseRateLimiter();
app.UseMiddleware<ApiKeyMiddleware>();
app.MapControllers();

Log.Information("miniCRM API Gateway starting on port 5080");
app.Run("http://0.0.0.0:5080");
