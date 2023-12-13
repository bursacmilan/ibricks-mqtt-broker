using ibricks_mqtt_broker.Model;

namespace ibricks_mqtt_broker.Services.Interface;

public interface IIbricksMessageInterpretor
{
    IbricksMessage Interpret(string messageString);
}