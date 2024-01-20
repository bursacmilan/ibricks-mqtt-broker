using ibricks_mqtt_broker.Model;
using ibricks_mqtt_broker.Services.Cello.FromCello.CommandParser;
using ibricks_mqtt_broker.Services.Interface;
using Microsoft.Extensions.Logging;

namespace ibricks_mqtt_broker.Services.Cello.FromCello;

public class IbricksMessageParserService(
    ILogger<IbricksMessageParserService> logger,
    ICelloStoreService celloStoreService,
    IMqttPublisherService mqttPublisherService,
    IMqttSubscriberService mqttSubscriberService,
    IIpMacService ipMacService,
    IUdpSenderService udpSenderService,
    IbricksBackgroundHandler backgroundHandler) : IIbricksMessageParserService
{
    public async Task ParseMessageAsync(IbricksMessage message)
    {
        logger.LogDebug("Parsing message with ID '{Id}' and command {Command}: '{Message}'", message.MessageId,
            message.Command, message.GetMessageAsString());

        IIbricksCommandParser? commandParser = message.Command.Name switch
        {
            nameof(IbricksMessageCommands.YHELO) => new YheloParser(logger, celloStoreService, ipMacService,
                udpSenderService),
            nameof(IbricksMessageCommands.YINFO) when message.GetAdditionalOrDefault<string>(IbricksMessageParts.V)
                ?.StartsWith("Hardware") ?? false => new YinfoParser(logger, celloStoreService),
            nameof(IbricksMessageCommands.LRCHG) => new LrchgRelayParser(logger, celloStoreService,
                mqttPublisherService, mqttSubscriberService),
            nameof(IbricksMessageCommands.LDCHG) => new LdchgDimmerParser(logger, celloStoreService,
                mqttPublisherService, mqttSubscriberService),
            nameof(IbricksMessageCommands.SICHG) => new SichgMeteoParser(logger, celloStoreService,
                mqttPublisherService, mqttSubscriberService),
            nameof(IbricksMessageCommands.BDCHG) => new BdchgClimateParser(logger, celloStoreService,
                mqttPublisherService, mqttSubscriberService),
            nameof(IbricksMessageCommands.ASCHG) => new AschgCoverParser(logger, celloStoreService,
                mqttPublisherService, mqttSubscriberService),
            nameof(IbricksMessageCommands.SSGES) => new SsgesClickParser(logger, celloStoreService,
                mqttPublisherService, backgroundHandler),
            _ => null
        };

        if (commandParser == null)
        {
            logger.LogInformation("Not supported command {Command}", message.Command.Name);
            return;
        }

        await commandParser.ParseAsync(message);
    }
}