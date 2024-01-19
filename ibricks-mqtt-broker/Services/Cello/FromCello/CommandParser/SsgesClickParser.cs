using ibricks_mqtt_broker.Model;
using ibricks_mqtt_broker.Model.DeviceState;
using ibricks_mqtt_broker.Services.Interface;
using Microsoft.Extensions.Logging;

namespace ibricks_mqtt_broker.Services.Cello.FromCello.CommandParser;

public class SsgesClickParser(ILogger logger, ICelloStoreService celloStoreService, IMqttPublisherService mqttPublisherService, IMqttSubscriberService mqttSubscriberService) : IIbricksCommandParser
{
    public async Task ParseAsync(IbricksMessage message)
    {
        var st = message.GetAdditionalOrDefault<string?>(IbricksMessageParts.ST);
        if (st == null)
        {
            logger.LogError("{ID}: SSGES no ST value found", message.MessageId);
            return;
        }
        
        var cello = await celloStoreService.TryGetCelloAsync(message.AddressFrom);
        if (cello == null)
        {
            logger.LogError("{ID}: Cello with address {Address} not found", message.MessageId, message.AddressFrom);
            return;
        }

        var channel = st.StartsWith("ClickRight") ? 1 : st.StartsWith("ClickLeft") ? 2 : -1;
        if (channel == -1)
        {
            logger.LogError("Could not parse ST-Value {ST} to channel", st);
            return;
        }

        var state = await celloStoreService.AddOrUpdateStateAsync(cello, channel, cello.EventStates, _ => { }, () =>
            new EventState
            {
                Channel = channel,
                CelloMacAddress = cello.Mac
            });

        await mqttPublisherService.PublishMessageAsync(state.GetMqttStateTopic(), EventState.Press,
            false);
    }
}