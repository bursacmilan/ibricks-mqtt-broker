using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using ibricks_mqtt_broker.Model;
using ibricks_mqtt_broker.Model.DeviceState;
using ibricks_mqtt_broker.Services.Interface;
using Microsoft.Extensions.Logging;

namespace ibricks_mqtt_broker.Services.Cello.ToCello.DeviceSateUpdater;

public class DimmerStateUpdater(
    ILogger<DimmerStateUpdater> logger,
    IUdpSenderService udpSenderService,
    IIpMacService ipMacService,
    ICelloStoreService celloStoreService) : IDeviceStateUpdater
{
    public async Task UpdateStateAsync(JsonNode deviceStateJson, bool isSingleValueJson, Model.Cello cello, int channel)
    {
        var additional = isSingleValueJson ? deviceStateJson["Additional"]?.Deserialize<string?>() : null;
        if (additional != null)
        {
            deviceStateJson[nameof(DimmerState.Value)] =
                additional.Equals("true", StringComparison.CurrentCultureIgnoreCase) ? 100 : 0;
        }

        var dimmerState = deviceStateJson.Deserialize<DimmerState>(JsonSerializerOptionsDefaults.IgnoreCase);
        if (dimmerState == null)
        {
            logger.LogError("Could not parse JSON {Json} to dimmerState", deviceStateJson.ToJsonString());
            return;
        }

        if (channel == DimmerState.GlobalDimmerChannel)
        {
            logger.LogDebug("This is a global dimmer. Only updating internally");
            await celloStoreService.AddOrUpdateStateAsync(cello, channel, cello.DimmerStates, state =>
            {
                state.Value = dimmerState.Value;
                state.IsOn = dimmerState.Value > 0;
            }, () => new DimmerState
            {
                CelloMacAddress = cello.Mac,
                Channel = channel,
                IsOn = dimmerState.Value > 0,
                Value = dimmerState.Value
            });

            return;
        }

        var dimmerMessage = new IbricksMessage
        {
            Channel = channel,
            Command = IbricksMessageCommands.LDSET,
            Nonce = IbricksMessageConstants.GetNonce(),
            Protocol = IbricksMessageConstants.KissProtocol,
            Type = IbricksMessageType.C,
            AddressFrom = ipMacService.GetMacAddress(),
            AddressTo = cello.Mac,
            AdditionalData = new Dictionary<string, string>
            {
                {
                    IbricksMessageParts.V.Name,
                    ((double) dimmerState.Value / 100).ToString(CultureInfo.InvariantCulture)
                },
                {
                    IbricksMessageParts.X.Name, IbricksMessageConstants.X
                }
            }
        };

        await udpSenderService.SendMessageAsync(cello.Ip, NetworkDefaults.UdpPort, dimmerMessage);
    }
}