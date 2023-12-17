using System.Text;

namespace ibricks_mqtt_broker.Model.DeviceState;

public class ClimateState() : DeviceState(DeviceStates.ClimateState)
{
    public decimal SetTo { get; set; }
    public string Mode => "auto";
    
    public override string GetYaml(Cello cello)
    {
        var meteo = cello.MeteoStates.Values.FirstOrDefault(c => c.Channel == Channel) ??
                    cello.MeteoStates.Values.FirstOrDefault();

        return GetJsonFromEmbeddedResource("Climate", GetMqttCommandTopic(), GetMqttStateTopic())
            .Replace("{state_meteostate}", meteo?.GetMqttStateTopic());
    }

    public override string GetHomeAssistantType() => "climate";
}