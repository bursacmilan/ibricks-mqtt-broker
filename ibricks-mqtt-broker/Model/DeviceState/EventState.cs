using System.Text.Json.Serialization;

namespace ibricks_mqtt_broker.Model.DeviceState;

public class EventState() : DeviceState(DeviceStates.EventState)
{
    public const string Press = "press";
    
    [JsonPropertyName("event_type")]
    public required string EventType { get; set; }
    
    public override string GetJson(Cello cello)
    {
        return GetJsonFromEmbeddedResource("Event", GetMqttCommandTopic(), GetMqttStateTopic());
    }
    
    public override string GetHomeAssistantType() => "event";
}