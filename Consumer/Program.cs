using Consumer.Consumers;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMassTransit(configurator =>
{
    configurator.AddConsumer<ExampleMessageConsumer>();

    configurator.UsingRabbitMq((rabbitContext, rabbitConfigurator) =>
    {
        rabbitConfigurator.Host(
            new Uri("amqps://swmagxbm:CloaBFaIESN-sqbjKWNh33gu8Ppyb3uB@sparrow.rmq.cloudamqp.com/swmagxbm"));

        rabbitConfigurator.ReceiveEndpoint(queueName: "example-message-queue",
            configureEndpoint: e => e.ConfigureConsumer<ExampleMessageConsumer>(rabbitContext));
    });
});

builder.Logging.ClearProviders();
builder.Logging.AddNLog();

var host = builder.Build();

host.Run();