using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using Steeltoe.Messaging.RabbitMQ.Extensions;
using Steeltoe.Messaging.RabbitMQ.Config;
using Connection = Steeltoe.Messaging.RabbitMQ.Connection;
using Steeltoe.Messaging.RabbitMQ.Listener.Exceptions;
using Microsoft.Extensions.Options;

namespace Messaging;
public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddSteeltoeMessaging(this IServiceCollection services)
    {
        services.AddRabbitServices(useJsonMessageConverter: true);
        services.AddRabbitAdmin();
        services.AddRabbitTemplate();
        return services;
    }

    public static IServiceCollection DeclareQueuesAndBindings(this IServiceCollection services)
    {
        services.AddRabbitQueue(provider =>
            new Queue(provider.GetSettings().QueueName, durable: true, exclusive: false, autoDelete: true, new Dictionary<string, object> { { "x-message-ttl", 1000 } }));

        services.AddRabbitExchange(provider => new TopicExchange(provider.GetSettings().ExchangeName, true, true));

        services.AddRabbitBinding(provider =>
        {
            var settings = provider.GetSettings();
            return new QueueBinding(settings.BindingName,
             settings.QueueName, settings.ExchangeName, settings.RoutingKey, null);
        });

        return services; 
    }

    private static RabbitMqSettings GetSettings(this IServiceProvider provider)
    {
        return provider.GetService<IOptions<RabbitMqSettings>>()?.Value ?? throw new Exception("RabbitMQSettings not configured");
    }
}
