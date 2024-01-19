using ibricks_mqtt_broker.Services.Interface;
using Microsoft.Extensions.Logging;
using MQTTnet;

namespace ibricks_mqtt_broker.Services.Mqtt.ToMqtt;

public class MqttPublisherService(ILogger<MqttPublisherService> logger, IMqttClientFactory mqttClientFactory) : IMqttPublisherService
{
    public async Task PublishMessageAsync(string topic, string payload, bool retain = true)
    {
        logger.LogDebug("Publishing mqtt message top topic {Topic}: {Message}", topic, payload);
        
        var mqttClientAsync = await mqttClientFactory.GetMqttClientAsync();
        var message = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(payload)
            .WithRetainFlag(retain)
            .Build();

        await mqttClientAsync.PublishAsync(message, CancellationToken.None);
        logger.LogDebug("Message to topic {Topic} published", topic);
    }
}