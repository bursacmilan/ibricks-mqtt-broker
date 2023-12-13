using ibricks_mqtt_broker.Infrastructure;
using ibricks_mqtt_broker.Services.Interface;
using Microsoft.Extensions.Options;

namespace ibricks_mqtt_broker.Services;

public class IpMacService(IOptionsMonitor<GlobalSettings> globalSettingsOptionsMonitor) : IIpMacService
{
    private const string Broadcast = "255.255.255.255";

    public string GetIp() => globalSettingsOptionsMonitor.CurrentValue.Ip;

    public string GetBroadcastIp() => Broadcast;

    public string GetMacAddress() => globalSettingsOptionsMonitor.CurrentValue.Mac;
}