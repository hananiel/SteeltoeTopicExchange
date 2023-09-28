using KitchenService;
using Messaging;
using Microsoft.AspNetCore.ResponseCompression;
using Steeltoe.Messaging.RabbitMQ.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMqSettings"));
builder.Services.DeclareQueuesAndBindings();

builder.Services.AddRabbitAdmin();
builder.Services.AddRabbitTemplate();

builder.Services.AddRabbitServices(true);
builder.Services.AddSingleton<RabbitReceiver>();
builder.Services.AddRabbitListeners<RabbitReceiver>();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});



builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});

var app = builder.Build();
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
