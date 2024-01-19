namespace ibricks_mqtt_broker.Services.Interface;

public interface IMqttPublisherService
{
    Task PublishMessageAsync(string topic, string payload, bool retain = true);
}