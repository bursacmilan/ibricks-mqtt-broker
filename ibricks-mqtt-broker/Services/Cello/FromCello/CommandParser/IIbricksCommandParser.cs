using ibricks_mqtt_broker.Model;

namespace ibricks_mqtt_broker.Services.Cello.FromCello.CommandParser;

public interface IIbricksCommandParser
{
    Task ParseAsync(IbricksMessage message);
}