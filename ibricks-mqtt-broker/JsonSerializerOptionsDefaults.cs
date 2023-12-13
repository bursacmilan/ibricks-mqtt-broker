using System.Text.Json;

namespace ibricks_mqtt_broker;

public static class JsonSerializerOptionsDefaults
{
    public static JsonSerializerOptions IgnoreCase = new()
    {
        PropertyNameCaseInsensitive = true
    };
}