using System.Text.Json;
using ibricks_mqtt_broker.Model;
using ibricks_mqtt_broker.Model.DeviceState;
using ibricks_mqtt_broker.Services.Interface;
using Microsoft.Extensions.Logging;

namespace ibricks_mqtt_broker.Services.Cello.FromCello.CommandParser;

public class SsgesClickParser(ILogger logger, ICelloStoreService celloStoreService, IMqttPublisherService mqttPublisherService, IIbricksBackgroundHandler ibricksBackgroundHandler) : IIbricksCommandParser
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

        if (st.Equals("FullTouch;XlongClick", StringComparison.InvariantCulture))
        {
            logger.LogDebug("Long click full touch triggered. Creating global event command");
            await mqttPublisherService.PublishMessageAsync("CELLOFULLTOUCH", JsonSerializer.Serialize(new EventState
                {
                    EventType = EventState.Press,
                    Channel = -1,
                    CelloMacAddress = "ALL"
                }),
                false);

            return;
        }

        if (st.Equals("FullTouch;Click", StringComparison.InvariantCultureIgnoreCase))
        {
            logger.LogDebug("Full touch pressed. Handling all channels");
            await HandleChannel(1, cello);
            await HandleChannel(2, cello);
            return;
        }

        if (st.Equals("WheelClockwise;1", StringComparison.InvariantCultureIgnoreCase))
        {
            logger.LogDebug("Wheel clockwise starting");
            await ibricksBackgroundHandler.RegisterBackgroundActivityAsync(cello, DeviceStates.EventState, "WHEEL", 350,
                async () =>
                {
                    await HandleWheel(cello);
                }, 10);

            return;
        }

        if (st.Equals("WheelStop", StringComparison.InvariantCultureIgnoreCase))
        {
            logger.LogDebug("Stopping wheel control");
            await ibricksBackgroundHandler.StopBackgroundActivityAsync(cello, DeviceStates.EventState, "WHEEL");
            return;
        }
        
        var channel = st.StartsWith("ClickRight") ? 1 : st.StartsWith("ClickLeft") ? 2 : -1;
        if (channel == -1)
        {
            logger.LogError("Could not parse ST-Value {ST} to channel", st);
            return;
        }

        await HandleChannel(channel, cello);
    }

    private async Task HandleChannel(int channel, Model.Cello cello)
    {
        var state = await celloStoreService.AddOrUpdateStateAsync(cello, channel, cello.EventStates,
            state => { state.EventType = EventState.Press; }, () =>
                new EventState
                {
                    EventType = EventState.Press,
                    Channel = channel,
                    CelloMacAddress = cello.Mac
                });

        await mqttPublisherService.PublishMessageAsync(state.GetMqttStateTopic(), JsonSerializer.Serialize(state),
            false);
    }
    
    private async Task HandleWheel(Model.Cello cello)
    {
        var state = await celloStoreService.AddOrUpdateStateAsync(cello, 1, cello.EventStates,
            state => { state.EventType = EventState.Wheel; }, () =>
                new EventState
                {
                    EventType = EventState.Wheel,
                    Channel = 1,
                    CelloMacAddress = cello.Mac
                });

        await mqttPublisherService.PublishMessageAsync(state.GetMqttStateTopic(), JsonSerializer.Serialize(state),
            false);
    }
}