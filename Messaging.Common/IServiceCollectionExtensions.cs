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
}
