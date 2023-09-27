using KitchenService;
using Messaging;
using Microsoft.AspNetCore.ResponseCompression;
using Steeltoe.Extensions.Configuration;
using Steeltoe.Messaging.RabbitMQ.Config;
using Steeltoe.Messaging.RabbitMQ.Extensions;
using Steeltoe.Messaging.RabbitMQ.Support.Converter;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>()
    .AddCommandLine(args);



builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMqSettings"));
builder.Services.SetUpRabbitMq();

var rabbitConfigSection = builder.Configuration.GetSection(RabbitOptions.PREFIX);
builder.Services.Configure<RabbitOptions>(rabbitConfigSection);

builder.Services.AddRabbitJsonMessageConverter();
builder.Services.AddRabbitServices();
builder.Services.AddRabbitAdmin();
builder.Services.AddRabbitTemplate();

builder.Services.AddSingleton<RabbitReceiver>();
builder.Services.AddRabbitListeners<RabbitReceiver>();
// Add services to the container.

builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});


builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseResponseCompression();
app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapHub<OrderHub>("/orderhub");
app.MapFallbackToPage("/_Host");

app.Run();
