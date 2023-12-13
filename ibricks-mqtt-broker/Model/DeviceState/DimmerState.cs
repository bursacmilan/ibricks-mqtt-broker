namespace ibricks_mqtt_broker.Model.DeviceState;

public class DimmerState() : DeviceState(DeviceStates.DimmerState)
{
    public int Value { get; set; }
    public bool IsOn { get; set; }
    
    public override string GetYaml(Cello cello, string name)
    {
        var yaml = GetYaml("Dimmer");
        yaml = yaml.Replace("{name}", name)
            .Replace("{state_dimmer}", GetMqttStateTopic())
            .Replace("{command_dimmer}", GetMqttCommandTopic());

        return yaml;
    }
}