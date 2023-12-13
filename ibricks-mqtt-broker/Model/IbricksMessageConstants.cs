namespace ibricks_mqtt_broker.Model;

public static class IbricksMessageConstants
{
    public const string KissProtocol = ".KISS";
    public const string UnknownAddressTo = "000000000000";
    public const string X = "123";
    public const char Delimiter = '|';

    public static string GetNonce()
    {
        return Random.Shared.Next(100000, 999999).ToString();
    }
}