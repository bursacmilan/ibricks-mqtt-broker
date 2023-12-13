using System.Text.Json;
using ibricks_mqtt_broker.Model;
using ibricks_mqtt_broker.Model.DeviceState;
using ibricks_mqtt_broker.Services.Interface;
using Microsoft.Extensions.Logging;

namespace ibricks_mqtt_broker.Services.Cello.FromCello.CommandParser;

public class LdchgDimmerParser(
    ILogger logger,
    ICelloStoreService celloStoreService,
    IMqttPublisherService mqttPublisherService,
    IMqttSubscriberService mqttSubscriberService) : IIbricksCommandParser
{
    public async Task ParseAsync(IbricksMessage message)
    {
        if (message.Channel == -1)
        {
            logger.LogWarning("{ID}: Channel is not set (-1)", message.MessageId);
            return;
        }

        var v = message.GetAdditionalOrDefault<double?>(IbricksMessageParts.V);
        if (v == null)
        {
            logger.LogError("{ID}: LDCHG no V value found", message.MessageId);
            return;
        }

        var cello = celloStoreService.TryGetCello(message.AddressFrom);
        if (cello == null)
        {
            logger.LogError("{ID}: Cello with address {Address} not found", message.MessageId, message.AddressFrom);
            return;
        }

        var value = (int) (v * 100);
        var state = cello.AddOrUpdateState(message.Channel, cello.DimmerStates, state =>
        {
            state.Value = value;
            state.IsOn = value > 0;
        }, () => new DimmerState
        {
            Value = value,
            IsOn = value > 0,
            Channel = message.Channel,
            CelloMacAddress = cello.Mac
        });

        await mqttPublisherService.PublishMessageAsync(state.GetMqttStateTopic(), JsonSerializer.Serialize(state));
        await mqttSubscriberService.SubscribeToTopicAsync(state.GetMqttCommandTopic());

        logger.LogDebug("{ID}: Dimmer for channel {Channel} updated to: Value {Value}", message.MessageId,
            message.Channel, value);
    }
}