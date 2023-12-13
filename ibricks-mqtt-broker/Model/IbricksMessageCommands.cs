// ReSharper disable InconsistentNaming

using Ardalis.SmartEnum;

namespace ibricks_mqtt_broker.Model;

public sealed class IbricksMessageCommands(string name, int value) : SmartEnum<IbricksMessageCommands>(name, value)
{
    public static IbricksMessageCommands YHELO = new(nameof(YHELO), 0);
    public static IbricksMessageCommands YINFO = new(nameof(YINFO), 1);
    public static IbricksMessageCommands YSCFG = new(nameof(YSCFG), 2);
    public static IbricksMessageCommands LRCHG = new(nameof(LRCHG), 3);
    public static IbricksMessageCommands LDCHG = new(nameof(LDCHG), 4);
    public static IbricksMessageCommands LDSET = new(nameof(LDSET), 5);
    public static IbricksMessageCommands ASCHG = new(nameof(ASCHG), 6);
    public static IbricksMessageCommands BDCHG = new(nameof(BDCHG), 7);
    public static IbricksMessageCommands SICHG = new(nameof(SICHG), 8);
    public static IbricksMessageCommands LRSET = new(nameof(LRSET), 9);
    public static IbricksMessageCommands BDSET = new(nameof(BDSET), 10);
    public static IbricksMessageCommands ASSET = new(nameof(ASSET), 11);
}