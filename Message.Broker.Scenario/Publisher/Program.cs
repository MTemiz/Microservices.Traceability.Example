using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Publisher.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMassTransit(configurator =>
{
    configurator.UsingRabbitMq((rabbitContext, rabbitConfigurator) =>
    {
        rabbitConfigurator.Host(new Uri("amqps://swmagxbm:CloaBFaIESN-sqbjKWNh33gu8Ppyb3uB@sparrow.rmq.cloudamqp.com/swmagxbm"));
    });
});

builder.Logging.ClearProviders();
builder.Logging.AddNLog();

builder.Services.AddHostedService<PublishMessageService>(provider =>
{
   using IServiceScope serviceScope = provider.CreateScope();

   IPublishEndpoint publishEndpoint = serviceScope.ServiceProvider.GetService<IPublishEndpoint>();
   
   var logger = serviceScope.ServiceProvider.GetService<ILogger<PublishMessageService>>();

   return new(publishEndpoint, logger);
});

var host = builder.Build();

host.Run();