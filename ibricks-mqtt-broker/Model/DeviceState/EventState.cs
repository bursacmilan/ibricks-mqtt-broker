namespace ibricks_mqtt_broker.Model.DeviceState;

public class EventState() : DeviceState(DeviceStates.EventState)
{
    public const string Press = "press";
    
    public override string GetJson(Cello cello)
    {
        return GetJsonFromEmbeddedResource("Event", GetMqttCommandTopic(), GetMqttStateTopic());
    }
    
    public override string GetHomeAssistantType() => "event";
}