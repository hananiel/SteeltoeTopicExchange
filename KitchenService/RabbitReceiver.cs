using Messaging;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Framing.Impl;
using Steeltoe.Messaging.RabbitMQ.Attributes;
using Steeltoe.Messaging.RabbitMQ.Config;
using Steeltoe.Messaging.RabbitMQ.Core;
using System.Text;
using System.Text.Json;

namespace KitchenService
{
    public class RabbitReceiver 
    {

        private readonly IHubContext<OrderHub> _orderHub;
        public RabbitReceiver( IHubContext<OrderHub> hub)
        {
            _orderHub = hub;
        }

        [DeclareExchange(Name = "${RabbitMqSettings:ExchangeName}", Type= "topic")]
        [DeclareQueue(Name = "${RabbitMqSettings:QueueName}")]
        [DeclareQueueBinding(Name = "${RabbitMqSettings:BindingName}", ExchangeName = "${RabbitMqSettings:ExchangeName}", QueueName = "${RabbitMqSettings:QueueName}", RoutingKey = "${RabbitMqSettings:RoutingKey}")]
        [RabbitListener("${RabbitMqSettings:QueueName}")]
        public void DoStuff(Order order)
        {
            Console.WriteLine("Success: Received Order"+ order );
            _orderHub.Clients.All.SendAsync("new-order", order);
        }
    }
}