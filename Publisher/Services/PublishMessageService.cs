using System.Diagnostics;
using System.Text.Json;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared;

namespace Publisher.Services;

public class PublishMessageService(IPublishEndpoint publishEndpoint, ILogger<PublishMessageService> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var correlationId = Guid.NewGuid();

        int i = 0;

        while (true)
        {
            ExampleMessage exampleMessage = new()
            {
                Text = $"{++i}. message"
            };

            Trace.CorrelationManager.ActivityId = correlationId;

            logger.LogDebug("Publisher log");

            await Console.Out.WriteLineAsync(
                $"{JsonSerializer.Serialize(exampleMessage)} - Correlation Id: {correlationId}");

            await publishEndpoint.Publish(exampleMessage,
                async publishContext =>
                {
                    publishContext.Headers.Set("CorrelationId", correlationId);
                });
            await Task.Delay(750);
        }
    }
}