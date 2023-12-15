namespace ibricks_mqtt_broker.Model.DeviceState;

public class RelayState() : DeviceState(DeviceStates.RelayState)
{
    public bool IsOn { get; set; }
    
    public override string GetYaml(Cello cello, string name)
    {
        var yaml = GetYaml("Relay");
        yaml = yaml.Replace("{name}", name)
            .Replace("{state_relay}", GetMqttStateTopic())
            .Replace("{command_relay}", GetMqttCommandTopic())
            .Replace("{area}", name.Split(" ").First())
            .Replace("{uid}", name.ToLower().Replace(" ", "-"));

        return yaml;
    }
}