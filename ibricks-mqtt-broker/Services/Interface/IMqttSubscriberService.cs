namespace ibricks_mqtt_broker.Services.Interface;

public interface IMqttSubscriberService
{
    Task SubscribeToTopicAsync(string topic);
}