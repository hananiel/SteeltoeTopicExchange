using KitchenService;
using Messaging;
using Microsoft.AspNetCore.ResponseCompression;
using Steeltoe.Messaging.RabbitMQ.Config;
using Steeltoe.Messaging.RabbitMQ.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>()
    .AddCommandLine(args);

///--------------from steeltoe hostbuilder
var rabbitConfigSection = builder.Configuration.GetSection(RabbitOptions.PREFIX);
builder.Services.Configure<RabbitOptions>(rabbitConfigSection);
builder.Services.AddRabbitServices();
builder.Services.AddRabbitAdmin();
builder.Services.AddRabbitTemplate();
///--------------


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

builder.Services.SetUpRabbitMq(builder.Configuration);
builder.Services.AddHostedService<RabbitReceiver>();

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
