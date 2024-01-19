using Ardalis.SmartEnum;

namespace ibricks_mqtt_broker.Model.DeviceState;

public sealed class DeviceStates : SmartEnum<DeviceStates>
{
    public static readonly DeviceStates DimmerState = new(nameof(DimmerState), 0);
    public static readonly DeviceStates RelayState = new(nameof(RelayState), 1);
    public static readonly DeviceStates MeteoState = new(nameof(MeteoState), 2);
    public static readonly DeviceStates ClimateState = new(nameof(ClimateState), 3);
    public static readonly DeviceStates CoverState = new(nameof(CoverState), 4);
    public static readonly DeviceStates EventState = new(nameof(EventState), 5);

    private DeviceStates(string name, int value) : base(name, value)
    {
    }
}