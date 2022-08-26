using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using Steeltoe.Messaging.RabbitMQ.Extensions;
using Steeltoe.Messaging.RabbitMQ.Config;
using Connection = Steeltoe.Messaging.RabbitMQ.Connection;
using Steeltoe.Messaging.RabbitMQ.Listener.Exceptions;

namespace Messaging;
public static class IServiceCollectionExtensions
{

    public static IServiceCollection SetUpRabbitMq(this IServiceCollection services, IConfiguration config)
    {
        var configSection = config.GetSection("RabbitMqSettings");

        var settings = new RabbitMqSettings();
        configSection.Bind(settings);

        // add the settings for later use by other classes via injection
        services.AddSingleton<RabbitMqSettings>(settings);
        services.AddRabbitQueue(new Queue(settings.QueueName, durable: true, exclusive: false, autoDelete: true, new Dictionary<string, object>
                {
                    { "x-message-ttl", 10000 }
                }));
        services.AddRabbitExchange(new TopicExchange(settings.ExchangeName, true, true));
        services.AddRabbitBindings(
            new QueueBinding(settings.BindingName,
            settings.QueueName, settings.ExchangeName, settings.RoutingKey, null));

        //services.AddSingleton<Connection.IConnectionFactory>(sp => new Connection.CachingConnectionFactory
        //{
        //    Host = settings.HostName,
        //    //DispatchConsumersAsync = true
        //});
        services.AddSingleton<IModel>(sp => sp.GetRabbitConnectionFactory().CreateConnection().CreateChannel());
        //services.AddSingleton(sp => sp.GetRequiredService<ModelFactory>());


        return services; 
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
