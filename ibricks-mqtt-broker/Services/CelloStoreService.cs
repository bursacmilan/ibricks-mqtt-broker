using ibricks_mqtt_broker.Database;
using ibricks_mqtt_broker.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ibricks_mqtt_broker.Services;

public class CelloStoreService(ILogger<CelloStoreService> logger, DatabaseContext dbContext) : ICelloStoreService
{
    public async Task AddOrUpdateCelloAsync(string ip, string mac, string description)
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
            await dbContext.SaveChangesAsync();
            return;
        }

        dbContext.Add(cello);
        await dbContext.SaveChangesAsync();
    }

    public async Task TryUpdateCelloAsync(string mac, Action<Model.Cello> action)
    {
        var cello = await TryGetCelloAsync(mac);
        if (cello == null)
            return;

        action(cello);
        await dbContext.SaveChangesAsync();
    }

    public async Task<Model.Cello?> TryGetCelloAsync(string mac)
    {
        var cello = await dbContext.Cellos.FirstOrDefaultAsync(c => c.Mac == mac);
        if (cello != null)
            return cello;

        logger.LogError("Could not get cello with mac {mac}", mac);
        return null;
    }

    public Task<Model.Cello[]> GetAllCellosAsync()
    {
        return dbContext.Cellos.AsNoTracking().ToArrayAsync();
    }

    public async Task<T> AddOrUpdateStateAsync<T>(Model.Cello cello, int channel, Dictionary<int, T> states,
        Action<T> updateState, Func<T> newState)
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
        await dbContext.SaveChangesAsync();
        return state;
    }

    public async Task<T> UpdateStateAsync<T>(Model.Cello cello, int channel, Dictionary<int, T> states,
        Action<T> updateState)
    {
        if (!states.TryGetValue(channel, out var existingState))
            throw new Exception($"No current state found for channel {channel} on cello {cello.Mac}");

        updateState(existingState);
        dbContext.Cellos.Update(cello);
        await dbContext.SaveChangesAsync();

        return existingState;
    }

    public Task<T?> GetCurrentStateAsync<T>(Model.Cello cello, int channel, Dictionary<int, T> states)
    {
        return Task.FromResult(states.GetValueOrDefault(channel));
    }
}