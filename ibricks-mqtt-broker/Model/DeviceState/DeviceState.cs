using System.Reflection;

namespace ibricks_mqtt_broker.Model.DeviceState;

public abstract class DeviceState(DeviceStates deviceState)
{
    public required int Channel { get; init; }
    public required string CelloMacAddress { get; init; }
    
    public string? DisplayName { get; set; }
    public string? UniqueId { get; set; }
    public DateTime? LastUpdate { get; set; }
    public bool Published { get; set; }

    private string? _mqttStateTopic;
    private string? _mqttCommandTopic;

    public string GetMqttStateTopic()
    {
        if (_mqttStateTopic != null)
            return _mqttStateTopic;

        _mqttStateTopic = $"{CelloMacAddress}/state/{deviceState.Name}/{Channel}".ToUpper();
        return _mqttStateTopic;
    }

    public string GetMqttCommandTopic()
    {
        if (_mqttCommandTopic != null)
            return _mqttCommandTopic;

        _mqttCommandTopic = $"{CelloMacAddress}/command/{deviceState.Name}/{Channel}".ToUpper();
        return _mqttCommandTopic;
    }

    public abstract string GetJson(Cello cello);
    public abstract string GetHomeAssistantType();

    protected string GetJsonFromEmbeddedResource(string name, string command, string state)
    {
        if (string.IsNullOrEmpty(DisplayName))
            throw new Exception("DisplayName is not set");

        if (string.IsNullOrEmpty(UniqueId))
            throw new Exception("UniqueId is not set");

        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"ibricks_mqtt_broker.Model.DeviceState.Json.{name}.json";

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
            return string.Empty;

        using var reader = new StreamReader(stream);
        var jsonContent = reader.ReadToEnd();

        return jsonContent.Replace("{state}", state)
            .Replace("{command}", command)
            .Replace("{name}", DisplayName)
            .Replace("{uid}", UniqueId);
    }
}