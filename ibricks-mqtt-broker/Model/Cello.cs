using ibricks_mqtt_broker.Model.DeviceState;

namespace ibricks_mqtt_broker.Model;

public class Cello
{
    public required string Ip { get; init; }
    public required string Description { get; init; }
    public required string Mac { get; init; }
    public IbricksHardwareInfo? HardwareInfo { get; set; }

    public Dictionary<int, RelayState> RelayStates { get; } = new();
    public Dictionary<int, DimmerState> DimmerStates { get; } = new();
    public Dictionary<int, MeteoState> MeteoStates { get; } = new();
    public Dictionary<int, ClimateState> ClimateStates { get; } = new();
    public Dictionary<int, CoverState> CoverStates { get; } = new();

    public T AddOrUpdateState<T>(int channel, Dictionary<int, T> states, Action<T> updateState, Func<T> newState)
    {
        if (states.TryGetValue(channel, out var existingState))
        {
            updateState(existingState);
            return existingState;
        }

        var state = newState();
        states[channel] = state;
        return state;
    }
    
    public T UpdateState<T>(int channel, Dictionary<int, T> states, Action<T> updateState)
    {
        if (states.TryGetValue(channel, out var existingState))
        {
            updateState(existingState);
            return existingState;
        }

        throw new Exception($"No current state found for channel {channel} on cello {Mac}");
    }
    
    public T? GetCurrentState<T>(int channel, Dictionary<int, T> states)
    {
        return states.GetValueOrDefault(channel);
    }
}