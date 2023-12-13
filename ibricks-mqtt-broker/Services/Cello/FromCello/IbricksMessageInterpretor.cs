using System.Text.Json;
using System.Web;
using ibricks_mqtt_broker.Model;
using ibricks_mqtt_broker.Services.Interface;
using Microsoft.Extensions.Logging;

namespace ibricks_mqtt_broker.Services.Cello.FromCello;

public class IbricksMessageInterpretor(ILogger<IbricksMessageInterpretor> logger) : IIbricksMessageInterpretor
{
    public IbricksMessage Interpret(string messageString)
    {
        logger.LogDebug("Interpreting new message: {Message}", messageString);
        var messageParts = messageString.Split(IbricksMessageConstants.Delimiter).Select(GetCleanedData).ToArray();

        logger.LogDebug("Message has total {Count} parts", messageParts.Length);
        
        var protocol = string.Empty;
        var addressFrom = string.Empty;
        var addressTo = string.Empty;
        var nonce = string.Empty;
        var type = IbricksMessageType.E;
        var command = IbricksMessageCommands.YHELO;
        var channel = -1;
        var additionalData = new Dictionary<string, string>();
        
        foreach (var messagePart in messageParts)
        {
            var keyValueArray = messagePart.Split('=');
            logger.LogDebug("Interpreting message part {Part} with total of {Total} sub-parts", messagePart,
                keyValueArray.Length);
            
            switch (keyValueArray.Length)
            {
                case 2:
                {
                    InterpretLength2(keyValueArray, additionalData, ref addressFrom, ref addressTo, ref nonce, ref channel);
                    break;
                }
                case > 2:
                {
                    InterpretLengthGreater2(keyValueArray, additionalData);
                    break;
                }
                default:
                {
                    InterpretLength1(keyValueArray, ref protocol, ref type, ref command);
                    break;
                }
            }
        }
        
        var message = new IbricksMessage
        {
            AddressFrom = addressFrom,
            AddressTo = addressTo,
            Channel = channel,
            Command = command,
            Nonce = nonce,
            Protocol = protocol,
            Type = type,
            AdditionalData = additionalData
        };

        logger.LogDebug("Message successfully interpreted to: {Message}", JsonSerializer.Serialize(message));
        return message;
    }

    private static void InterpretLength1(string[] keyValueArray, ref string protocol, ref IbricksMessageType type,
        ref IbricksMessageCommands command)
    {
        var key = keyValueArray[0];
        switch (key)
        {
            case IbricksMessageConstants.KissProtocol:
                protocol = IbricksMessageConstants.KissProtocol;
                break;
            default:
                if (Enum.TryParse<IbricksMessageType>(key, out var parsedType))
                {
                    type = parsedType;
                    break;
                }
                            
                if (IbricksMessageCommands.TryFromName(key, out var parsedMessageCommand))
                    command = parsedMessageCommand;

                break;
        }
    }

    private static void InterpretLengthGreater2(string[] keyValueArray, Dictionary<string, string> additionalData)
    {
        var key = keyValueArray[0];
        var value = string.Join('=', keyValueArray.Skip(1));
        additionalData[key] = value;
    }

    private static void InterpretLength2(string[] keyValueArray, Dictionary<string, string> additionalData, ref string addressFrom,
        ref string addressTo, ref string nonce, ref int channel)
    {
        var key = keyValueArray[0];
        var value = keyValueArray[1];

        switch (key)
        {
            case nameof(IbricksMessageParts.AF):
                addressFrom = value;
                break;
            case nameof(IbricksMessageParts.AT):
                addressTo = value;
                break;
            case nameof(IbricksMessageParts.N):
                nonce = value;
                break;
            case nameof(IbricksMessageParts.CH):
                if (int.TryParse(value, out var parsedChannel))
                    channel = parsedChannel;
                break;
            default:
                additionalData[key] = value;
                break;
        }
    }

    private static string GetCleanedData(string data)
    {
        return HttpUtility.UrlDecode(data.Replace("\r", ""));
    }
}