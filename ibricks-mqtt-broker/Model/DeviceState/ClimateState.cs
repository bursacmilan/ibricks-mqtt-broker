using System.Text;

namespace ibricks_mqtt_broker.Model.DeviceState;

public class ClimateState() : DeviceState(DeviceStates.ClimateState)
{
    public decimal SetTo { get; set; }
    public string Mode => "auto";
    
    public override string GetYaml(Cello cello, string name)
    {
        var meteoForChannel = cello.GetCurrentState(Channel, cello.MeteoStates);
        if (meteoForChannel == null)
            return string.Empty;

        var yaml = GetYaml("Climate");
        yaml = yaml.Replace("{name}", name).Replace("{state_meteostate}", meteoForChannel.GetMqttStateTopic())
            .Replace("{state_climatestate}", GetMqttStateTopic())
            .Replace("{command_climatestate}", GetMqttCommandTopic())
            .Replace("{area}", name.Split(" ").First())
            .Replace("{uid}", name.ToLower().Replace(" ", "-"));

        return yaml;
    }
}