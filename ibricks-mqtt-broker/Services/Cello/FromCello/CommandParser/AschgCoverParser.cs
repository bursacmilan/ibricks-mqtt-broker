using System.Text.Json;
using ibricks_mqtt_broker.Model;
using ibricks_mqtt_broker.Model.DeviceState;
using ibricks_mqtt_broker.Services.Interface;
using Microsoft.Extensions.Logging;

namespace ibricks_mqtt_broker.Services.Cello.FromCello.CommandParser;

public class AschgCoverParser(ILogger logger, ICelloStoreService celloStoreService, IMqttPublisherService mqttPublisherService, IMqttSubscriberService mqttSubscriberService) : IIbricksCommandParser
{
    public async Task ParseAsync(IbricksMessage message)
    {
        if (message.Channel == -1)
        {
            logger.LogWarning("{ID}: Channel is not set (-1)", message.MessageId);
            return;
        }
            
        var cmd = message.GetAdditionalOrDefault<string?>(IbricksMessageParts.CMD);
        if (cmd == null)
        {
            logger.LogError("{ID}: ASCHG no CMD value found", message.MessageId);
            return;
        }

        var supportedCommands = new[] {"UP", "DN", "HL", "ST"};
        if (!supportedCommands.Contains(cmd))
        {
            logger.LogDebug("{ID}: Command {Cmd} is not supported", message.MessageId, cmd);
            return;
        }
        
        var cello = celloStoreService.TryGetCello(message.AddressFrom);
        if (cello == null)
        {
            logger.LogError("{ID}: Cello with address {Address} not found", message.MessageId, message.AddressFrom);
            return;
        }

        decimal? currentPosition = null;
        decimal? tiltPosition = null;
        string? movingState = null;
        
        switch (cmd)
        {
            case "UP":
                movingState = CoverState.MovingOpening;
                break;
            case "DN":
                movingState = CoverState.MovingClosing;
                break;
            case "HL":
            case "ST":
                if (cmd == "ST")
                    movingState = CoverState.MovingStopped;

                currentPosition = message.GetAdditionalOrDefault<decimal?>(IbricksMessageParts.H);
                tiltPosition = message.GetAdditionalOrDefault<decimal?>(IbricksMessageParts.L);
                break;
        }

        var currentPositionInt = currentPosition != null ? ConvertPosition(currentPosition) : null;
        var tiltPositionInt = tiltPosition != null ? ConvertPosition(tiltPosition) : null;
        
        var state = celloStoreService.AddOrUpdateState(cello, message.Channel, cello.CoverStates, state =>
        {
            state.CurrentPosition = currentPositionInt ?? state.CurrentPosition;
            state.CurrentMovingState = movingState ?? state.CurrentMovingState;
            state.TiltPosition = tiltPositionInt ?? state.TiltPosition;
        }, () => new CoverState
        {
            CurrentPosition = currentPositionInt ?? 50,
            CurrentMovingState = movingState ?? CoverState.MovingStopped,
            TiltPosition = tiltPositionInt ?? 0,
            Channel = message.Channel,
            CelloMacAddress = cello.Mac
        });

        await mqttPublisherService.PublishMessageAsync(state.GetMqttStateTopic(), JsonSerializer.Serialize(state));
        await mqttSubscriberService.SubscribeToTopicAsync(state.GetMqttCommandTopic());

        logger.LogDebug(
            "{ID}: Cover for channel {Channel} updated to: Position {Position} - Moving {Moving} - Tilt {Tilt}",
            message.MessageId,
            message.Channel, state.CurrentPosition, state.CurrentMovingState, state.TiltPosition);
    }
    
    private int? ConvertPosition(decimal? number)
    {
        if (number == null)
            return null;
        
        return (int) Math.Round(number.Value * 100, MidpointRounding.ToEven);
    }
}