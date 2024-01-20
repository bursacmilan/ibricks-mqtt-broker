namespace ibricks_mqtt_broker.Model.DeviceState;

public class DimmerState() : DeviceState(DeviceStates.DimmerState)
{
    public const int GlobalDimmerChannel = 100;
    
    public int Value { get; set; }
    public bool IsOn { get; set; }
    
    public override string GetJson(Cello cello)
    {
        return GetJsonFromEmbeddedResource("Dimmer", GetMqttCommandTopic(), GetMqttStateTopic());
    }
    
    public override string GetHomeAssistantType() => "light";
}