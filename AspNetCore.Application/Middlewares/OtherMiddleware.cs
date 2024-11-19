using NLog;

namespace AspNetCore.Application.Middlewares;

public class OtherMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context, ILogger<OtherMiddleware> logger)
    {
        var headerCorrelationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault();

        var contextCorrelationId = context.Items["CorrelationId"].ToString();

        NLog.MappedDiagnosticsContext.Set("CorrelationId", headerCorrelationId);
        
        logger.LogDebug("Other Middleware Invoked");
        
        await next(context);
    }
}