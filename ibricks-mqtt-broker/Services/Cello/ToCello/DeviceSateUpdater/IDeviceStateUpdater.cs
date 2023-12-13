using System.Text.Json.Nodes;

namespace ibricks_mqtt_broker.Services.Cello.ToCello.DeviceSateUpdater;

public interface IDeviceStateUpdater
{
    Task UpdateStateAsync(JsonNode deviceStateJson, bool isSingleValueJson, Model.Cello cello, int channel);
}