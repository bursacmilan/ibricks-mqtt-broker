namespace ibricks_mqtt_broker.Services.Interface;

public interface ICelloStoreService
{
    void AddOrUpdateCello(string ip, string mac, string description);
    void TryUpdateCello(string mac, Action<Model.Cello> action);
    Model.Cello? TryGetCello(string mac);
    Model.Cello[] GetAllCellos();
    T AddOrUpdateState<T>(Model.Cello cello, int channel, Dictionary<int, T> states, Action<T> updateState, Func<T> newState);
    T UpdateState<T>(Model.Cello cello, int channel, Dictionary<int, T> states, Action<T> updateState);
    T? GetCurrentState<T>(Model.Cello cello, int channel, Dictionary<int, T> states);
}