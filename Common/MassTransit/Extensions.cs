using System.Reflection;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.MassTransit;

public static class Extensions
{
    public static IServiceCollection AddMassTransitWithRabbitMq(this IServiceCollection services)
    {
        services.AddMassTransit(config =>
        {
            //Consumer Config
            config.AddConsumers(Assembly.GetEntryAssembly());
            
            //Producer Config
            config.UsingRabbitMq((context, configurator) =>
            {
                var configuration = context.GetService<IConfiguration>();
                var serviceName = configuration.GetSection("ServiceSettings")["Name"];
                var rabbitMqHost = configuration.GetSection("RabbitMQSettings")["Host"];
                
                configurator.Host(rabbitMqHost);
                configurator.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serviceName,false));
            });
        });
        return services;
    }
}