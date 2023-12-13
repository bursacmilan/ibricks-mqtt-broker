namespace ibricks_mqtt_broker.Model;

public record IbricksHardwareInfo(int R, int S, int H, int D)
{
    public override string ToString()
    {
        return $"R: {R}, S: {S}, H: {H}, D: {D}";
    }
}