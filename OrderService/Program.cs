using Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using OrderService;
using Steeltoe.Extensions.Configuration;
using Steeltoe.Messaging.RabbitMQ.Config;
using Steeltoe.Messaging.RabbitMQ.Extensions;
using Steeltoe.Messaging.RabbitMQ.Host;
using Steeltoe.Messaging.RabbitMQ.Support.Converter;
using static Steeltoe.Messaging.RabbitMQ.Connection.CorrelationData;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>()
.AddCommandLine(args)
.Build();

builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMqSettings"));

//builder.Services.AddRabbitQueues()
/////--------------from steeltoe hostbuilder
var rabbitConfigSection = builder.Configuration.GetSection(RabbitOptions.PREFIX);
builder.Services.Configure<RabbitOptions>(rabbitConfigSection);

builder.Services.AddRabbitJsonMessageConverter();
builder.Services.AddRabbitServices();
builder.Services.AddRabbitAdmin();
builder.Services.AddRabbitTemplate();
///--------------

builder.Services.DeclareQueuesAndBindings();
builder.Services.AddSingleton<RabbitSender>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var orderIdSeed = 1;
app.MapPost("/waffleOrder", (RabbitSender rabbitSender, [FromBody] Order order) =>
{
    if (order.Id is 0)
    {
        order = new Order().Seed(orderIdSeed);
        orderIdSeed++;
    }
    rabbitSender.PublishMessage<Order>(order, "order.cookwaffle");
});

app.Run();

