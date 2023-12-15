using ibricks_mqtt_broker.Infrastructure;
using ibricks_mqtt_broker.Services;
using ibricks_mqtt_broker.Services.Cello.FromCello;
using ibricks_mqtt_broker.Services.Cello.ToCello;
using ibricks_mqtt_broker.Services.Cello.ToCello.DeviceSateUpdater;
using ibricks_mqtt_broker.Services.Interface;
using ibricks_mqtt_broker.Services.Mqtt;
using ibricks_mqtt_broker.Services.Mqtt.FromMqtt;
using ibricks_mqtt_broker.Services.Mqtt.ToMqtt;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();

builder.Services.AddSingleton<ICelloStoreService, CelloStoreService>();
builder.Services.AddSingleton<IIbricksMessageInterpretor, IbricksMessageInterpretor>();
builder.Services.AddSingleton<IIbricksMessageParserService, IbricksMessageParserService>();
builder.Services.AddSingleton<IIpMacService, IpMacService>();
builder.Services.AddSingleton<IUdpSenderService, UdpSenderService>();
builder.Services.AddSingleton<IIbricksStateUpdaterService, IbricksStateUpdaterService>();
builder.Services.AddSingleton<IMqttPublisherService, MqttPublisherService>();
builder.Services.AddSingleton<IMqttClientFactory, MqttClientFactory>();
builder.Services.AddSingleton<IMqttSubscriberService, MqttSubscriberService>();

builder.Services.AddStateUpdater();

builder.Services.AddHostedService<UdpHostedService>();

builder.Services.Configure<GlobalSettings>(builder.Configuration.GetSection("Global"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();