namespace ibricks_mqtt_broker.Model.DeviceState;

public class DimmerState() : DeviceState(DeviceStates.DimmerState)
{
    public int Value { get; set; }
    public bool IsOn { get; set; }
    
    public override string GetYaml(Cello cello)
    {
        return GetJsonFromEmbeddedResource("Dimmer", GetMqttCommandTopic(), GetMqttStateTopic());
    }
    
    public override string GetHomeAssistantType() => "light";
}