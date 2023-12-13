using System.Text.Json.Nodes;
using ibricks_mqtt_broker.Infrastructure;
using ibricks_mqtt_broker.Model.DeviceState;
using ibricks_mqtt_broker.Services.Cello.ToCello.DeviceSateUpdater;
using ibricks_mqtt_broker.Services.Interface;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ibricks_mqtt_broker.Services.Cello.ToCello;

public class IbricksStateUpdaterService(
    ILogger<IbricksStateUpdaterService> logger,
    IIpMacService ipMacService,
    ICelloStoreService celloStoreService,
    IUdpSenderService udpSenderService,
    IOptionsMonitor<GlobalSettings> globalSettingsOptionsMonitor) : IIbricksStateUpdaterService
{
    public async Task UpdateStateAsync(JsonNode deviceStateJson, bool isSingleValueJson, DeviceStates stateType,
        int channel, string celloMacAddress)
    {
        logger.LogDebug("Parsing state of type {Type} for cello with mac {Mac} and channel {Channel}", stateType,
            channel, celloMacAddress);

        var cello = celloStoreService.TryGetCello(celloMacAddress);
        if (cello == null)
        {
            logger.LogError("Could not get cello with mac {Mac}", celloMacAddress);
            return;
        }

        IDeviceStateUpdater? stateUpdater = stateType.Name switch
        {
            nameof(DeviceStates.DimmerState) => new DimmerStateUpdater(logger, udpSenderService, ipMacService),
            nameof(DeviceStates.RelayState) => new RelayStateUpdater(logger, udpSenderService, ipMacService),
            nameof(DeviceStates.ClimateState) => new ClimateStateUpdater(logger, udpSenderService, ipMacService),
            nameof(DeviceStates.CoverState) => new CoverStateUpdater(logger, udpSenderService, ipMacService,
                globalSettingsOptionsMonitor),
            _ => null
        };

        if (stateUpdater == null)
        {
            logger.LogWarning("Device of type {Type} is not supported", stateType.Name);
            return;
        }

        await stateUpdater.UpdateStateAsync(deviceStateJson, isSingleValueJson, cello, channel);
    }
}