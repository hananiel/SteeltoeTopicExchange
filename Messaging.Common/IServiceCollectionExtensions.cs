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

    public static IServiceCollection SetUpRabbitMq(this IServiceCollection services)//, IConfiguration config)
    {
        //var configSection = config.GetSection("RabbitMqSettings");

        //var settings = new RabbitMqSettings();
        //configSection.Bind(settings);


        // add the settings for later use by other classes via injection
        // services.AddSingleton<RabbitMqSettings>(settings);
        // services.AddRabbitQueue(new Queue(settings.QueueName, durable: true, exclusive: false, autoDelete: true, new Dictionary<string, object>
        //{
        //    { "x-message-ttl", 10000 }
        //}));
        services.AddRabbitQueue(provider =>
            new Queue(provider.GetSettings().QueueName, durable: true, exclusive: false, autoDelete: true, new Dictionary<string, object> { { "x-message-ttl", 1000 } }));

        services.AddRabbitExchange(provider => new TopicExchange(provider.GetSettings().ExchangeName, true, true));

        services.AddRabbitBinding(provider =>
        {
            var settings = provider.GetSettings();
           return new QueueBinding(settings.BindingName,
            settings.QueueName, settings.ExchangeName, settings.RoutingKey, null);
        });

        //builder.Services.AddRabbitServices();
        //builder.Services.AddRabbitAdmin();
        return services; 
    }

    private static RabbitMqSettings GetSettings(this IServiceProvider provider)
    {
        return provider.GetService<IOptions<RabbitMqSettings>>()?.Value ?? throw new Exception("RabbitMQSettings not configured");
    }
    //public class ModelFactory : IDisposable
    //{
    //    private readonly Connection.IConnection _connection;
    //    private readonly RabbitMqSettings _settings;
    //    private readonly IQueue _queue;
    //    public ModelFactory(
    //        Connection.IConnectionFactory connectionFactory,
    //        RabbitMqSettings settings,
    //        IQueue queue)
    //    {
    //        _settings = settings;
    //        _connection = connectionFactory.CreateConnection();
    //        _queue = queue;
    //    }

    //    public IModel CreateChannel()
    //    {
    //        var channel = _connection.CreateChannel();
    //        channel.
    //        //channel.QueueDeclare(_settings.QueueName, durable: false, exclusive: false, autoDelete: true, new Dictionary<string, object>
    //        //    {
    //        //        { "x-message-ttl", 10000 }
    //        //    });
    //        //channel.ExchangeDeclare(exchange: _settings.ExchangeName, type: _settings.ExchangeType);
    //        return channel;
    //    }

    //    public void Dispose()
    //    {
    //        _connection.Dispose();
    //    }
    //}
}
