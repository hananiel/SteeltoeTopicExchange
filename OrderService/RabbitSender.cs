using Messaging;
using Microsoft.Extensions.Options;
using OrderService;
using RabbitMQ.Client;
using Steeltoe.Messaging;
using Steeltoe.Messaging.RabbitMQ.Config;
using Steeltoe.Messaging.RabbitMQ.Core;
using System.Text;
using System.Text.Json;

public class RabbitSender
{
    private readonly RabbitTemplate _template;
    private readonly RabbitMqSettings _rabbitSettings;

    public RabbitSender(
        IOptions<RabbitMqSettings> rabbitSettings, 
        RabbitTemplate template)
    {
        _template = template;
        _rabbitSettings = rabbitSettings.Value;
    }

    public void PublishMessage<T>(T entity, string key) where T : class
    {
        _template.ConvertAndSend(exchange: _rabbitSettings.ExchangeName,
             routingKey: _rabbitSettings.RoutingKey,
             message: entity);
        
        Console.WriteLine(" [x] Sent '{0}':'{1}'", key, JsonSerializer.Serialize(entity)); 
    }
}