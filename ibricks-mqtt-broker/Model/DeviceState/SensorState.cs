using System.Text.Json.Serialization;

namespace ibricks_mqtt_broker.Model.DeviceState;

public class SensorState() : DeviceState(DeviceStates.SensorState)
{
    public const string Clockwise = "clockwise";
    public const string Counterclockwise = "counterclockwise";
    public const string Idle = "idle";
    
    [JsonPropertyName("state")]
    public required string State { get; set; }
    
    public override string GetJson(Cello cello)
    {
        return GetJsonFromEmbeddedResource("Sensor", GetMqttCommandTopic(), GetMqttStateTopic());
    }
    
    public override string GetHomeAssistantType() => "sensor";
}