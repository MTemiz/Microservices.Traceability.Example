using Microsoft.Extensions.Primitives;

namespace AspNetCore.Application.Middlewares;

public class CorrelationIdMiddleware(RequestDelegate next)
{
    const string correlationIdHeaderName = "X-Correlation-ID";

    public async Task Invoke(HttpContext context, ILogger<CorrelationIdMiddleware> logger)
    {
        string correlationId = Guid.NewGuid().ToString();

        if (context.Request.Headers.TryGetValue(correlationIdHeaderName, out StringValues _correlationId))
        {
            correlationId = _correlationId;
        }
        else
        {
            context.Request.Headers.Add(correlationIdHeaderName, correlationId);
        }

        NLog.MappedDiagnosticsContext.Set("CorrelationId", correlationId);

        logger.LogDebug("Asp.Net Core App. CorrelationId Log Example");

        context.Response.OnStarting(() =>
        {
            if (!context.Response.Headers.TryGetValue(correlationIdHeaderName, out _))
            {
                context.Response.Headers.Add(correlationIdHeaderName, correlationId);
            }
            
            return Task.CompletedTask;
        });
        
        // passing correlation id value to next middleware
        context.Items["CorrelationId"] = correlationId;
        
        await next(context);
    }
}