using System.Text.Json.Nodes;
using ibricks_mqtt_broker.Model.DeviceState;
using ibricks_mqtt_broker.Services.Cello.ToCello.DeviceSateUpdater;
using ibricks_mqtt_broker.Services.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ibricks_mqtt_broker.Services.Cello.ToCello;

public class IbricksStateUpdaterService(
    ILogger<IbricksStateUpdaterService> logger,
    ICelloStoreService celloStoreService,
    IServiceProvider serviceProvider) : IIbricksStateUpdaterService
{
    public async Task UpdateStateAsync(JsonNode deviceStateJson, bool isSingleValueJson, DeviceStates stateType,
        int channel, string celloMacAddress)
    {
        logger.LogDebug("Parsing state of type {Type} for cello with mac {Mac} and channel {Channel}", stateType,
            channel, celloMacAddress);

        var cello = await celloStoreService.TryGetCelloAsync(celloMacAddress);
        if (cello == null)
        {
            logger.LogError("Could not get cello with mac {Mac}", celloMacAddress);
            return;
        }

        IDeviceStateUpdater? stateUpdater = stateType.Name switch
        {
            nameof(DeviceStates.DimmerState) => serviceProvider.GetRequiredService<DimmerStateUpdater>(),
            nameof(DeviceStates.RelayState) => serviceProvider.GetRequiredService<RelayStateUpdater>(),
            nameof(DeviceStates.ClimateState) => serviceProvider.GetRequiredService<ClimateStateUpdater>(),
            nameof(DeviceStates.CoverState) => serviceProvider.GetRequiredService<CoverStateUpdater>(),
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