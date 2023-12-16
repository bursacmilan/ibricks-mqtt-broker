using System.Text.Json.Nodes;
using ibricks_mqtt_broker.Model.DeviceState;
using ibricks_mqtt_broker.Services.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;

namespace ibricks_mqtt_broker.Services.Mqtt.FromMqtt;

public class MqttSubscriberService(
    ILogger<MqttSubscriberService> logger,
    IMqttClientFactory mqttClientFactory,
    IServiceProvider serviceProvider) : IMqttSubscriberService
{
    private IMqttClient? _mqttClient;
    private readonly List<string> _subscribedTopics = [];

    public async Task SubscribeToTopicAsync(string topic)
    {
        logger.LogDebug("Subscribing to mqtt topic {Topic}", topic);

        if (_mqttClient == null)
        {
            _mqttClient = await mqttClientFactory.GetMqttClientAsync();
            _mqttClient.ApplicationMessageReceivedAsync += HandleIncomingMessageAsync;
            mqttClientFactory.OnReconnect += () => SubscribeToTopicsInternalAsync(_mqttClient);
        }

        if (_subscribedTopics.Contains(topic))
        {
            logger.LogDebug("Topic {Topic} already subscribed", topic);
            return;
        }

        _subscribedTopics.Add(topic);
        await SubscribeToTopicsInternalAsync(_mqttClient);
        logger.LogDebug("Subscribed to topic {Topic}", topic);
    }

    private async Task HandleIncomingMessageAsync(MqttApplicationMessageReceivedEventArgs e)
    {
        logger.LogDebug("Handling incoming mqtt message {Message}", e.ApplicationMessage.ConvertPayloadToString());

        var topic = e.ApplicationMessage.Topic;
        if (string.IsNullOrEmpty(topic))
        {
            logger.LogDebug("Topic is null or empty");
            return;
        }

        var splitted = topic.Split('/');
        if (splitted.Length < 4)
        {
            logger.LogDebug("Topic {Topic} has less than 4 parts", topic);
            return;
        }

        var channelAsString = splitted[^1];
        var stateTypeAsString = splitted[^2];
        var celloMacAddress = splitted[^4];

        if (!DeviceStates.TryFromName(stateTypeAsString, true, out var stateType))
        {
            logger.LogError("Could not parse device state {State}", stateTypeAsString);
            return;
        }

        if (!int.TryParse(channelAsString, out var channel))
        {
            logger.LogError("Could not parse channel {Channel} to int", channel);
            return;
        }

        var payload = e.ApplicationMessage.ConvertPayloadToString();
        if (string.IsNullOrEmpty(payload))
        {
            logger.LogDebug("Payload for topic {Topic} is null or empty", topic);
            return;
        }

        var isSimplifiedJson = false;
        JsonNode? jsonNode;
        try
        {
            jsonNode = JsonNode.Parse(payload);
            if (jsonNode?.Root.AsObject() == null)
                jsonNode = null;
        }
        catch (Exception)
        {
            logger.LogDebug("Could not parse payload to json. Adding as additional property");
            jsonNode = JsonNode.Parse($"{{\"Additional\":\"{payload}\"}}");
            isSimplifiedJson = true;
        }

        if (jsonNode == null)
        {
            logger.LogError("Could not parse payload {Payload} to json", payload);
            return;
        }

        jsonNode[nameof(DimmerState.Channel)] ??= channel;
        jsonNode[nameof(DimmerState.CelloMacAddress)] ??= celloMacAddress;

        try
        {
            using var scope = serviceProvider.CreateScope();
            var ibricksStateUpdaterService = scope.ServiceProvider.GetRequiredService<IIbricksStateUpdaterService>();
            await ibricksStateUpdaterService.UpdateStateAsync(jsonNode, isSimplifiedJson, stateType, channel,
                celloMacAddress);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error while processing state");
        }
    }

    private async Task SubscribeToTopicsInternalAsync(IMqttClient mqttClient)
    {
        var mqttSubscribeOptions = new MqttFactory().CreateSubscribeOptionsBuilder();
        foreach (var topic in _subscribedTopics)
        {
            mqttSubscribeOptions.WithTopicFilter(
                f => { f.WithTopic(topic); });
        }

        await mqttClient.SubscribeAsync(mqttSubscribeOptions.Build(), CancellationToken.None);
    }
}