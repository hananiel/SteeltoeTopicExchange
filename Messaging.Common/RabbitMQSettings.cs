namespace Messaging;
public class RabbitMqSettings
{
    public string HostName { get; set; }
    public string ExchangeName { get; set; }
    public string QueueName { get; set; }
    public string RoutingKey { get; set; }
    public string BindingName { get; set; }

    public string ExchangeType { get; set; }

}

