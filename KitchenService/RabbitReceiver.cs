using Messaging;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Framing.Impl;
using Steeltoe.Messaging.RabbitMQ.Attributes;
using Steeltoe.Messaging.RabbitMQ.Core;
using System.Text;
using System.Text.Json;

namespace KitchenService
{
    public class RabbitReceiver //: IHostedService
    {
        private readonly RabbitMqSettings _rabbitSettings;
        private readonly IModel _channel;
        private readonly RabbitTemplate _template;
        private readonly IHubContext<OrderHub> _orderHub;
        public RabbitReceiver(
            IOptions<RabbitMqSettings> rabbitSettings,
            IHubContext<OrderHub> hub,
            RabbitTemplate template)
        {
            _rabbitSettings = rabbitSettings.Value;
            //_channel = channel;
            _orderHub = hub;
            _template = template;
        }


      
        [RabbitListener(Binding = "${RabbitMqSettings:BindingName}")]
        public void DoStuff(Order order)
        {
            Console.WriteLine("Success: Received Order"+ order );
           // var order = JsonSerializer.Deserialize<Order>(orderMessage);
            _orderHub.Clients.All.SendAsync("new-order", order);
            //_channel.QueueBind(queue: _rabbitSettings.QueueName,
            //                  exchange: _rabbitSettings.ExchangeName,
            //                  routingKey: _rabbitSettings.RoutingKey);


            //var consumerAsync = new AsyncEventingBasicConsumer(_channel);
            ////_template.AddListener();

            //consumerAsync.Received += async (_, ea) =>
            //{
            //    var body = ea.Body.ToArray();
            //    var message = Encoding.UTF8.GetString(body);
            //    var order = JsonSerializer.Deserialize<Order>(message);
            //    await _orderHub.Clients.All.SendAsync("new-order", order);

            //    _channel.BasicAck(ea.DeliveryTag, false);
            //};

            //_channel.BasicConsume(queue: _rabbitSettings.QueueName,
            //                     autoAck: false,
            //                     consumer: consumerAsync);
        }
    }
}