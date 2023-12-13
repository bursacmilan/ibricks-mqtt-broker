namespace ibricks_mqtt_broker.Services.Interface;

public interface ICelloStoreService
{
    void AddOrUpdateCello(string ip, string mac, string description);
    void TryUpdateCello(string mac, Action<Model.Cello> action);
    Model.Cello? TryGetCello(string mac);
    Model.Cello[] GetAllCellos();
}