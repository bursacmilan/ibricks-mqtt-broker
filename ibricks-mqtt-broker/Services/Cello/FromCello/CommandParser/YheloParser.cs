using ibricks_mqtt_broker.Model;
using ibricks_mqtt_broker.Services.Interface;
using Microsoft.Extensions.Logging;

namespace ibricks_mqtt_broker.Services.Cello.FromCello.CommandParser;

public class YheloParser(
    ILogger logger,
    ICelloStoreService celloStoreService,
    IIpMacService ipMacService,
    IUdpSenderService udpSenderService) : IIbricksCommandParser
{
    public async Task ParseAsync(IbricksMessage message)
    {
        var ip = message.GetAdditionalOrDefault<string>(IbricksMessageParts.IP);
        var description = message.GetAdditionalOrDefault<string>(IbricksMessageParts.DESC);

        if (string.IsNullOrEmpty(ip) || string.IsNullOrEmpty(description))
        {
            logger.LogDebug("{ID}: Could not parse YHELO because IP or description is null or empty",
                message.MessageId);

            return;
        }

        logger.LogDebug("{ID}: Adding or updating cello. IP: {IP} AddressFrom: {AF}, Description: {Desc}",
            message.MessageId, ip,
            message.AddressFrom, description);

        celloStoreService.AddOrUpdateCello(ip, message.AddressFrom, description);

        var deviceStateMessage = new IbricksMessage
        {
            Channel = -1,
            Command = IbricksMessageCommands.YSCFG,
            Nonce = IbricksMessageConstants.GetNonce(),
            Protocol = IbricksMessageConstants.KissProtocol,
            Type = IbricksMessageType.C,
            AddressFrom = ipMacService.GetMacAddress(),
            AddressTo = message.AddressFrom,
            AdditionalData = new Dictionary<string, string>
            {
                {IbricksMessageParts.CFG.Name, "GetCurrentState"},
                {IbricksMessageParts.V.Name, "1"},
                {IbricksMessageParts.X.Name, IbricksMessageConstants.X}
            }
        };

        logger.LogDebug("Sending YSCFG message to cello with mac {Mac}", message.AddressFrom);
        await udpSenderService.SendMessageAsync(ip, NetworkDefaults.UdpPort, deviceStateMessage);
    }
}