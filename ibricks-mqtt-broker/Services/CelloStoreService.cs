using ibricks_mqtt_broker.Database;
using ibricks_mqtt_broker.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ibricks_mqtt_broker.Services;

public class CelloStoreService(ILogger<CelloStoreService> logger, DatabaseContext dbContext) : ICelloStoreService
{
    public void AddOrUpdateCello(string ip, string mac, string description)
    {
        var cello = new Model.Cello
        {
            Description = description,
            Mac = mac,
            Ip = ip
        };

        if (dbContext.Cellos.AsNoTracking().FirstOrDefault(c => c.Mac == mac) != null)
        {
            dbContext.Update(cello);
            dbContext.SaveChanges();
            return;
        }
        
        dbContext.Add(cello);
        dbContext.SaveChanges();
    }
    
    public void TryUpdateCello(string mac, Action<Model.Cello> action)
    {
        var cello = TryGetCello(mac);
        if (cello == null)
            return;
        
        action(cello);
        dbContext.SaveChanges();
    }

    public Model.Cello? TryGetCello(string mac)
    {
        var cello = dbContext.Cellos.FirstOrDefault(c => c.Mac == mac);        
        if (cello != null) 
            return cello;
        
        logger.LogError("Could not get cello with mac {mac}", mac);
        return null;
    }

    public Model.Cello[] GetAllCellos()
    {
        return dbContext.Cellos.AsNoTracking().ToArray();
    }
    
    public T AddOrUpdateState<T>(Model.Cello cello, int channel, Dictionary<int, T> states, Action<T> updateState, Func<T> newState)
    {
        if (states.TryGetValue(channel, out var state))
        {
            updateState(state);
        }
        else
        {
            state = newState();
            states[channel] = state;
        }

        dbContext.Cellos.Update(cello);
        dbContext.SaveChanges();
        return state;
    }
    
    public T UpdateState<T>(Model.Cello cello, int channel, Dictionary<int, T> states, Action<T> updateState)
    {
        if (!states.TryGetValue(channel, out var existingState))
            throw new Exception($"No current state found for channel {channel} on cello {cello.Mac}");

        updateState(existingState);
        dbContext.Cellos.Update(cello);
        dbContext.SaveChanges();
        
        return existingState;
    }
    
    public T? GetCurrentState<T>(Model.Cello cello, int channel, Dictionary<int, T> states)
    {
        return states.GetValueOrDefault(channel);
    }
}