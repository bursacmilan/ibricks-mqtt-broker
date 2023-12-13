namespace ibricks_mqtt_broker.Model.DeviceState;

public class MeteoState() : DeviceState(DeviceStates.MeteoState)
{
    public decimal Current { get; set; }
    
    public override string GetYaml(Cello cello, string name)
    {
        var yaml = GetYaml("Meteo");
        yaml = yaml.Replace("{name}", name)
            .Replace("{state_meteo}", GetMqttStateTopic())
            .Replace("{command_meteo}", GetMqttCommandTopic());

        return yaml;
    }
}