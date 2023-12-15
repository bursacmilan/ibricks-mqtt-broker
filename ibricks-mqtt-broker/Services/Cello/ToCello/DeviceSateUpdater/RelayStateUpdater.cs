using System.Text.Json;
using System.Text.Json.Nodes;
using ibricks_mqtt_broker.Model;
using ibricks_mqtt_broker.Model.DeviceState;
using ibricks_mqtt_broker.Services.Interface;
using Microsoft.Extensions.Logging;

namespace ibricks_mqtt_broker.Services.Cello.ToCello.DeviceSateUpdater;

public class RelayStateUpdater(ILogger<RelayStateUpdater> logger, IUdpSenderService udpSenderService, IIpMacService ipMacService) : IDeviceStateUpdater
{
    public async Task UpdateStateAsync(JsonNode deviceStateJson, bool isSingleValueJson, Model.Cello cello, int channel)
    {
        var additional = isSingleValueJson ? deviceStateJson["Additional"]?.Deserialize<string?>() : null;
        if (additional != null)
        {
            deviceStateJson[nameof(RelayState.IsOn)] =
                additional.Equals("true", StringComparison.CurrentCultureIgnoreCase) ? true : false;
        }
        
        var relayState = deviceStateJson.Deserialize<RelayState>(JsonSerializerOptionsDefaults.IgnoreCase);
        if (relayState == null)
        {
            logger.LogError("Could not parse JSON {Json} to relayState", deviceStateJson.ToJsonString());
            return;
        }
                
        var relayMessage = new IbricksMessage
        {
            Channel = channel,
            Command = IbricksMessageCommands.LRSET,
            Nonce = IbricksMessageConstants.GetNonce(),
            Protocol = IbricksMessageConstants.KissProtocol,
            Type = IbricksMessageType.C,
            AddressFrom = ipMacService.GetMacAddress(),
            AddressTo = cello.Mac,
            AdditionalData = new Dictionary<string, string>
            {
                {
                    IbricksMessageParts.ST.Name, (relayState.IsOn ? 1 : 0).ToString()
                },
                {
                    IbricksMessageParts.X.Name, IbricksMessageConstants.X
                }
            }
        };
                
        await udpSenderService.SendMessageAsync(cello.Ip, NetworkDefaults.UdpPort, relayMessage);
    }
}