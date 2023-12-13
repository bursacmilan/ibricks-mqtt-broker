using MQTTnet.Client;

namespace ibricks_mqtt_broker.Services.Interface;

public interface IMqttClientFactory
{
    Task<IMqttClient> GetMqttClientAsync();
    event Func<Task>? OnReconnect;
}