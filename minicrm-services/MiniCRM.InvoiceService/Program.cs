using MiniCRM.InvoiceService;
using MiniCRM.InvoiceService.Controllers;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new() { Title = "InvoiceService", Version = "v1" }));

builder.Services.AddHttpClient<InvoiceApiClient>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseSerilogRequestLogging();
app.MapControllers();

Log.Information("MiniCRM.InvoiceService starting on port 5084");
app.Run("http://0.0.0.0:5084");
