using System.Text.Json.Nodes;
using ibricks_mqtt_broker.Model.DeviceState;

namespace ibricks_mqtt_broker.Services.Interface;

public interface IIbricksStateUpdaterService
{
    Task UpdateStateAsync(JsonNode deviceStateJson, bool isSingleValueJson, DeviceStates stateType, int channel, string celloMacAddress);
}