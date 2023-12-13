using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using ibricks_mqtt_broker.Infrastructure;
using ibricks_mqtt_broker.Model;
using ibricks_mqtt_broker.Model.DeviceState;
using ibricks_mqtt_broker.Services.Interface;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ibricks_mqtt_broker.Services.Cello.ToCello.DeviceSateUpdater;

public class CoverStateUpdater(ILogger logger, IUdpSenderService udpSenderService, IIpMacService ipMacService, IOptionsMonitor<GlobalSettings> globalSettingsOptinsMonitor)
    : IDeviceStateUpdater
{
    public async Task UpdateStateAsync(JsonNode deviceStateJson, bool isSingleValueJson, Model.Cello cello, int channel)
    {
        CoverState? coverState;
        var tiltDisabled = globalSettingsOptinsMonitor.CurrentValue.DisableTilt?.Contains(cello.Mac) ?? false;
        
        var additionalCover = isSingleValueJson ? deviceStateJson["Additional"]?.Deserialize<string?>() : null;
        if (isSingleValueJson && int.TryParse(additionalCover, out var additionalCoverAsInt))
        {
            deviceStateJson[nameof(CoverState.TiltPosition)] = additionalCoverAsInt;
            
            coverState = deviceStateJson.Deserialize<CoverState>(JsonSerializerOptionsDefaults.IgnoreCase);
            if (coverState != null)
                coverState.CurrentPosition = -1;
        }
        else
        {
            coverState = deviceStateJson.Deserialize<CoverState>(JsonSerializerOptionsDefaults.IgnoreCase);
            if (coverState != null)
                coverState.TiltPosition = -1;
        }

        if (coverState == null)
        {
            logger.LogError("Could not parse JSON {Json} to coverState", deviceStateJson.ToJsonString());
            return;
        }

        var coverMessage = new IbricksMessage
        {
            Channel = channel,
            Command = IbricksMessageCommands.ASSET,
            Nonce = IbricksMessageConstants.GetNonce(),
            Protocol = IbricksMessageConstants.KissProtocol,
            Type = IbricksMessageType.C,
            AddressFrom = ipMacService.GetMacAddress(),
            AddressTo = cello.Mac,
            AdditionalData = new Dictionary<string, string>
            {
                {
                    IbricksMessageParts.CMD.Name, "HL"
                },
                {
                    IbricksMessageParts.H.Name, ConvertNumberToCelloValue(coverState.CurrentPosition)
                },
                {
                    IbricksMessageParts.L.Name,
                    tiltDisabled == false ? ConvertNumberToCelloValue(coverState.TiltPosition) : (-1).ToString()
                },
                {
                    IbricksMessageParts.X.Name, IbricksMessageConstants.X
                }
            }
        };

        await udpSenderService.SendMessageAsync(cello.Ip, NetworkDefaults.UdpPort, coverMessage);
    }

    private string ConvertNumberToCelloValue(int number)
    {
        return ((decimal) number / 100).ToString(CultureInfo.InvariantCulture);
    }
}