namespace ibricks_mqtt_broker.Model.DeviceState;

public class RelayState() : DeviceState(DeviceStates.RelayState)
{
    public bool IsOn { get; set; }
    
    public override string GetYaml(Cello cello)
    {
        return GetJsonFromEmbeddedResource("Relay", GetMqttCommandTopic(), GetMqttStateTopic());
    }
    
    public override string GetHomeAssistantType() => "light";
}