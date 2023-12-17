namespace ibricks_mqtt_broker.Model.DeviceState;

public class CoverState() : DeviceState(DeviceStates.CoverState)
{
    public static string MovingOpening = "opening";
    public static string MovingClosing = "closing";
    public static string MovingStopped = "stopped";
    
    public int CurrentPosition { get; set; }
    public int TiltPosition { get; set; }
    public string CurrentMovingState { get; set; } = MovingStopped;
    
    public override string GetJson(Cello cello)
    {
        return GetJsonFromEmbeddedResource("Cover", GetMqttCommandTopic(), GetMqttStateTopic());
    }
    
    public override string GetHomeAssistantType() => "cover";
}