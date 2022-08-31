using Messaging;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Framing.Impl;
using Steeltoe.Messaging;
using Steeltoe.Messaging.RabbitMQ.Attributes;
using Steeltoe.Messaging.RabbitMQ.Core;
using Steeltoe.Messaging.RabbitMQ.Listener;
using System.Text;
using System.Text.Json;

namespace KitchenService
{
    public class RabbitReceiver :IMessageListener
    {
        private readonly IHubContext<OrderHub> _orderHub;
        public RabbitReceiver(IHubContext<OrderHub> hub)
        {
            _orderHub = hub;
        }

        public AcknowledgeMode ContainerAckMode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void OnMessage(IMessage message)
        {
            var payload = Encoding.UTF8.GetString((byte[])message.Payload);
            var order = JsonSerializer.Deserialize<Order>(payload);
            Task.Run(async () => await(_orderHub.Clients.All.SendAsync("new-order", order)));
        }

        public void OnMessageBatch(List<IMessage> messages)
        {
            foreach (var message in messages)
            {
                OnMessage(message);
            }
        }

        //[RabbitListener(Binding = "waffle.binding")]
        //public async Task ReceiveMessage(string message)
        //{
        //    var order = JsonSerializer.Deserialize<Order>(message);
        //    await _orderHub.Clients.All.SendAsync("new-order", order);

        //}
    }
}