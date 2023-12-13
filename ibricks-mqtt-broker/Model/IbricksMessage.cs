using System.Text;

namespace ibricks_mqtt_broker.Model;

public class IbricksMessage
{
    public required string Protocol { get; init; }
    public required string AddressFrom { get; init; }
    public required string AddressTo { get; init; }
    public required string Nonce { get; init; }
    public required IbricksMessageType Type { get; init; }
    public required IbricksMessageCommands Command { get; init; }
    public required int Channel { get; init; }
    public Dictionary<string, string>? AdditionalData { get; init; }
    public string MessageId { get; } = Random.Shared.Next().ToString();
    
    public override string ToString()
    {
        return GetMessageAsString();
    }

    public string GetMessageAsString()
    {
        var messageBuilder = new StringBuilder();

        AddPart(messageBuilder, null, Protocol, false)
            .AddPart(messageBuilder, IbricksMessageParts.AF.Name, AddressFrom)
            .AddPart(messageBuilder, IbricksMessageParts.AT.Name, AddressTo)
            .AddPart(messageBuilder, IbricksMessageParts.N.Name, Nonce)
            .AddPart(messageBuilder, null, Type.ToString())
            .AddPart(messageBuilder, null, Command.ToString());
        
        if (Channel != -1)
            AddPart(messageBuilder, IbricksMessageParts.CH.Name, Channel.ToString());

        if (AdditionalData == null)
            return messageBuilder.ToString();
        
        foreach (var item in AdditionalData)
            AddPart(messageBuilder, item.Key, item.Value);
        
        return messageBuilder.ToString();
    }

    public T? GetAdditionalOrDefault<T>(IbricksMessageParts messagePart)
    {
        if (AdditionalData == null || !AdditionalData.TryGetValue(messagePart.Name, out var value))
            return default;

        var convertTo = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
        return (T) Convert.ChangeType(value, convertTo);
    }
    
    public T GetAdditional<T>(IbricksMessageParts messagePart)
    {
        if (AdditionalData == null || !AdditionalData.TryGetValue(messagePart.Name, out var value))
            throw new Exception($"Could not find additional data with key {messagePart.Name}");
        
        var convertTo = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
        return (T) Convert.ChangeType(value, convertTo);
    }

    private IbricksMessage AddPart(StringBuilder messageBuilder, string? name, string value, bool addDelimiter = true)
    {
        if (addDelimiter)
            messageBuilder.Append(IbricksMessageConstants.Delimiter);
        
        if (name == null)
            messageBuilder.Append(value);
        else
            messageBuilder.Append($"{name}={value}");

        return this;
    }
}