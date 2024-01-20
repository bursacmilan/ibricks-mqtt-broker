using ibricks_mqtt_broker.Model.DeviceState;

namespace ibricks_mqtt_broker.Services.Interface;

public interface IIbricksBackgroundHandler
{
    Task RegisterBackgroundActivityAsync(Model.Cello cello, DeviceStates deviceState, string identifier,
        int delayInMs, Func<Task> action, int maxRounds);

    Task StopBackgroundActivityAsync(Model.Cello cello, DeviceStates deviceState, string identifier);
}