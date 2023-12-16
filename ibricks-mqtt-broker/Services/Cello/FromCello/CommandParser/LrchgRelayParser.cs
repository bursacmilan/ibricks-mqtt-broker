using System.Text.Json;
using ibricks_mqtt_broker.Model;
using ibricks_mqtt_broker.Model.DeviceState;
using ibricks_mqtt_broker.Services.Interface;
using Microsoft.Extensions.Logging;

namespace ibricks_mqtt_broker.Services.Cello.FromCello.CommandParser;

public class LrchgRelayParser(
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

        var st = message.GetAdditionalOrDefault<int?>(IbricksMessageParts.ST);
        if (st == null)
        {
            logger.LogError("{ID}: LRCHG no ST value found", message.MessageId);
            return;
        }

        var cello = await celloStoreService.TryGetCelloAsync(message.AddressFrom);
        if (cello == null)
        {
            logger.LogError("{ID}: Cello with address {Address} not found", message.MessageId, message.AddressFrom);
            return;
        }

        var isOn = st == 1;
        var state = await celloStoreService.AddOrUpdateStateAsync(cello, message.Channel, cello.RelayStates, state => { state.IsOn = isOn; }, () =>
            new RelayState
            {
                IsOn = isOn,
                Channel = message.Channel,
                CelloMacAddress = cello.Mac
            });

        await mqttPublisherService.PublishMessageAsync(state.GetMqttStateTopic(), JsonSerializer.Serialize(state));
        await mqttSubscriberService.SubscribeToTopicAsync(state.GetMqttCommandTopic());

        logger.LogDebug("{ID}: Relay for channel {Channel} updated to: IsOn {IsOn}", message.MessageId, message.Channel,
            isOn);
    }
}