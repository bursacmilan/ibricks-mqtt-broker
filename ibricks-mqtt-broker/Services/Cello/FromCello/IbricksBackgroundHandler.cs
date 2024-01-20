using System.Collections.Concurrent;
using ibricks_mqtt_broker.Model.DeviceState;
using ibricks_mqtt_broker.Services.Interface;

namespace ibricks_mqtt_broker.Services.Cello.FromCello;

public class IbricksBackgroundHandler : IIbricksBackgroundHandler
{
    private readonly ConcurrentDictionary<string, CancellationTokenSource> _cancellationTokens = new();
    
    public async Task RegisterBackgroundActivityAsync(Model.Cello cello, DeviceStates deviceState, string identifier, int delayInMs,
        Func<Task> action, int maxRounds)
    {
        var id = GetId(cello, deviceState, identifier);
        
        var cancellationTokenSource = new CancellationTokenSource();
        _cancellationTokens.AddOrUpdate(id, cancellationTokenSource, (_, _) => cancellationTokenSource);

        var round = 0;
        while (round < maxRounds && !cancellationTokenSource.IsCancellationRequested)
        {
            await action();
            await Task.Delay(delayInMs, cancellationTokenSource.Token);
            round++;
        }

        _cancellationTokens.Remove(id, out _);
    }

    public async Task StopBackgroundActivityAsync(Model.Cello cello, DeviceStates deviceState, string identifier)
    {
        var id = GetId(cello, deviceState, identifier);
        if (_cancellationTokens.TryGetValue(id, out var cancellationToken))
            await cancellationToken.CancelAsync();
    }

    private string GetId(Model.Cello cello, DeviceStates deviceState, string identifier)
    {
        return cello.Mac + deviceState.Name + identifier;
    }
}