using ibricks_mqtt_broker.Model;

namespace ibricks_mqtt_broker.Services.Interface;

public interface IIbricksMessageParserService
{
    Task ParseMessageAsync(IbricksMessage message);
}