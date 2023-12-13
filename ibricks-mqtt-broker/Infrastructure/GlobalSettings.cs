namespace ibricks_mqtt_broker.Infrastructure;

public class GlobalSettings
{
    private readonly string _mac = string.Empty;

    public required string Ip { get; init; }

    public required string Mac
    {
        get => _mac;
        init => _mac = value.Replace(":", "").ToUpper();
    }

    public string[]? DisableTilt { get; set; }

    public MqttSettings? MqttSettings { get; set; }
}

public class MqttSettings
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Host { get; set; }
}