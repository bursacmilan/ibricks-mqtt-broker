using ibricks_mqtt_broker.Model.DeviceState;

namespace ibricks_mqtt_broker.Services.Interface;

public interface ICelloStoreService
{
    Task AddOrUpdateCelloAsync(string ip, string mac, string description);
    Task TryUpdateCelloAsync(string mac, Action<Model.Cello> action);
    Task<Model.Cello?> TryGetCelloAsync(string mac);
    Task<Model.Cello[]> GetAllCellosAsync();

    Task<T> AddOrUpdateStateAsync<T>(Model.Cello cello, int channel, Dictionary<int, T> states, Action<T> updateState,
        Func<T> newState) where T : DeviceState;

    Task<T> UpdateStateAsync<T>(Model.Cello cello, int channel, Dictionary<int, T> states, Action<T> updateState)
        where T : DeviceState;

    Task<T?> GetCurrentStateAsync<T>(Model.Cello cello, int channel, Dictionary<int, T> states) where T : DeviceState;
}