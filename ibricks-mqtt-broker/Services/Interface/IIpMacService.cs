namespace ibricks_mqtt_broker.Services.Interface;

public interface IIpMacService
{
    string GetIp();

    string GetBroadcastIp();

    string GetMacAddress();
}