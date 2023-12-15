using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using ibricks_mqtt_broker.Model;
using ibricks_mqtt_broker.Model.DeviceState;
using ibricks_mqtt_broker.Services.Interface;
using Microsoft.Extensions.Logging;

namespace ibricks_mqtt_broker.Services.Cello.ToCello.DeviceSateUpdater;

public class ClimateStateUpdater(ILogger<ClimateStateUpdater> logger, IUdpSenderService udpSenderService, IIpMacService ipMacService) : IDeviceStateUpdater
{
    public async Task UpdateStateAsync(JsonNode deviceStateJson, bool isSingleValueJson, Model.Cello cello, int channel)
    {
        if (isSingleValueJson)
        {
            logger.LogWarning("ClimateStateUpdater does not support single value json");
            return;
        }
        
        var climateState = deviceStateJson.Deserialize<ClimateState>(JsonSerializerOptionsDefaults.IgnoreCase);
        if (climateState == null)
        {
            logger.LogError("Could not parse JSON {Json} to climateState", deviceStateJson.ToJsonString());
            return;
        }
                
        var climateMessage = new IbricksMessage
        {
            Channel = channel,
            Command = IbricksMessageCommands.BDSET,
            Nonce = IbricksMessageConstants.GetNonce(),
            Protocol = IbricksMessageConstants.KissProtocol,
            Type = IbricksMessageType.C,
            AddressFrom = ipMacService.GetMacAddress(),
            AddressTo = cello.Mac,
            AdditionalData = new Dictionary<string, string>
            {
                {
                    IbricksMessageParts.U.Name, "CEL"
                },
                {
                    IbricksMessageParts.V.Name, climateState.SetTo.ToString(CultureInfo.InvariantCulture)
                },
                {
                    IbricksMessageParts.X.Name, IbricksMessageConstants.X
                }
            }
        };
                
        await udpSenderService.SendMessageAsync(cello.Ip, NetworkDefaults.UdpPort, climateMessage);
    }
}