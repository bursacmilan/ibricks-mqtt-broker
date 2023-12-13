using System.Reflection;

namespace ibricks_mqtt_broker.Model.DeviceState;

public abstract class DeviceState(DeviceStates deviceState)
{
    public required int Channel { get; init; }
    public required string CelloMacAddress { get; init; }

    private string? _mqttStateTopic; 
    private string? _mqttCommandTopic; 
    
    public string GetMqttStateTopic()
    {
        if(_mqttStateTopic != null)
            return _mqttStateTopic;

        _mqttStateTopic = $"{CelloMacAddress}/state/{deviceState.Name}/{Channel}".ToUpper();
        return _mqttStateTopic;
    }
    
    public string GetMqttCommandTopic()
    {
        if(_mqttCommandTopic != null)
            return _mqttCommandTopic;

        _mqttCommandTopic = $"{CelloMacAddress}/command/{deviceState.Name}/{Channel}".ToUpper();
        return _mqttCommandTopic;
    }

    public abstract string GetYaml(Cello cello, string name);

    protected string GetYaml(string name)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"ibricks_mqtt_broker.Model.DeviceState.Yaml.{name}.yaml";

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
            return string.Empty;
        
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}