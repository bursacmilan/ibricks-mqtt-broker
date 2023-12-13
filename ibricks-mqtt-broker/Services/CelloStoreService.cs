using System.Collections.Concurrent;
using ibricks_mqtt_broker.Services.Interface;
using Microsoft.Extensions.Logging;

namespace ibricks_mqtt_broker.Services;

public class CelloStoreService(ILogger<CelloStoreService> logger) : ICelloStoreService
{
    private readonly ConcurrentDictionary<string, Model.Cello> _cellos = new();

    public void AddOrUpdateCello(string ip, string mac, string description)
    {
        _cellos.AddOrUpdate(mac, _ => new Model.Cello
        {
            Description = description,
            Mac = mac,
            Ip = ip
        }, (_, _) => new Model.Cello
        {
            Description = description,
            Mac = mac,
            Ip = ip
        });
    }
    
    public void TryUpdateCello(string mac, Action<Model.Cello> action)
    {
        var cello = TryGetCello(mac);
        if (cello == null)
            return;
        
        action(cello);
    }

    public Model.Cello? TryGetCello(string mac)
    {
        if (_cellos.TryGetValue(mac, out var cello)) 
            return cello;
        
        logger.LogError("Could not get cello with mac {mac}", mac);
        return null;
    }

    public Model.Cello[] GetAllCellos()
    {
        return _cellos.Values.ToArray();
    }
}