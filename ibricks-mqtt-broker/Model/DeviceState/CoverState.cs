namespace ibricks_mqtt_broker.Model.DeviceState;

public class CoverState() : DeviceState(DeviceStates.CoverState)
{
    public static string MovingOpening = "opening";
    public static string MovingClosing = "closing";
    public static string MovingStopped = "stopped";
    
    public int CurrentPosition { get; set; }
    public int TiltPosition { get; set; }
    public string CurrentMovingState { get; set; } = MovingStopped;
    
    public override string GetYaml(Cello cello, string name)
    {
        var yaml = GetYaml("Cover");
        yaml = yaml.Replace("{name}", name)
            .Replace("{state_cover}", GetMqttStateTopic())
            .Replace("{command_cover}", GetMqttCommandTopic())
            .Replace("{area}", name.Split(" ").First())
            .Replace("{uid}", name.ToLower().Replace(" ", "-"));

        return yaml;
    }
}