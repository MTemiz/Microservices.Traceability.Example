using AspNetCore.Application.Middlewares;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Logging.ClearProviders();
builder.Host.UseNLog();

var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<OtherMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/", (HttpContext context, ILogger<Program> logger) =>
{
    var headerCorrelationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault();

    var contextCorrelationId = context.Items["CorrelationId"].ToString();

    NLog.MappedDiagnosticsContext.Set("CorrelationId", headerCorrelationId);
        
    logger.LogDebug("Minimal API Invoked");
});

app.Run();