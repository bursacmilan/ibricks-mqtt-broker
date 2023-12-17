namespace ibricks_mqtt_broker.Model.DeviceState;

public class MeteoState() : DeviceState(DeviceStates.MeteoState)
{
    public decimal Current { get; set; }
    
    public override string GetYaml(Cello cello)
    {
        return GetJsonFromEmbeddedResource("Meteo", GetMqttCommandTopic(), GetMqttStateTopic());
    }
    
    public override string GetHomeAssistantType() => "sensor";
}