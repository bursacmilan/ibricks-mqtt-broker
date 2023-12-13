using ibricks_mqtt_broker.Model;

namespace ibricks_mqtt_broker.Services.Interface;

public interface IUdpSenderService
{
    Task SendMessageAsync(string ipAddress, int port, IbricksMessage message);
}