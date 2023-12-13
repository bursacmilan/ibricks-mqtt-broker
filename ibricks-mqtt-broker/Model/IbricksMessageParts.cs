// ReSharper disable InconsistentNaming

using Ardalis.SmartEnum;

namespace ibricks_mqtt_broker.Model;

public sealed class IbricksMessageParts : SmartEnum<IbricksMessageParts>
{
    public static readonly IbricksMessageParts AF = new(nameof(AF), 0);
    public static readonly IbricksMessageParts AT = new(nameof(AT), 1);
    public static readonly IbricksMessageParts N = new(nameof(N), 2);
    public static readonly IbricksMessageParts CH = new(nameof(CH), 3);
    public static readonly IbricksMessageParts IP = new(nameof(IP), 4);
    public static readonly IbricksMessageParts DESC = new(nameof(DESC), 5);
    public static readonly IbricksMessageParts V = new(nameof(V), 6);
    public static readonly IbricksMessageParts X = new(nameof(X), 7);
    public static readonly IbricksMessageParts ST = new(nameof(ST), 8);
    public static readonly IbricksMessageParts CFG = new(nameof(CFG), 9);
    public static readonly IbricksMessageParts U = new(nameof(U), 10);
    public static readonly IbricksMessageParts CMD = new(nameof(CMD), 11);
    public static readonly IbricksMessageParts H = new(nameof(H), 12);
    public static readonly IbricksMessageParts L = new(nameof(L), 13);

    private IbricksMessageParts(string name, int value) : base(name, value)
    {
    }
}