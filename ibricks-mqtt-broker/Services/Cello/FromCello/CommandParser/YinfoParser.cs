using ibricks_mqtt_broker.Model;
using ibricks_mqtt_broker.Services.Interface;
using Microsoft.Extensions.Logging;

namespace ibricks_mqtt_broker.Services.Cello.FromCello.CommandParser;

public class YinfoParser(ILogger logger, ICelloStoreService celloStoreService) : IIbricksCommandParser
{
    public Task ParseAsync(IbricksMessage message)
    {
        var hardwareInfo = message.GetAdditional<string>(IbricksMessageParts.V).Split("/")[0];
        var parsedHardwareInfo = ParseHardwareInfo(message, hardwareInfo);

        celloStoreService.TryUpdateCello(message.AddressFrom,
            cello => cello.HardwareInfo = parsedHardwareInfo);

        logger.LogDebug("{ID}: Hardware info updated to: {HInfo}", message.MessageId, hardwareInfo);
        return Task.CompletedTask;
    }

    private IbricksHardwareInfo ParseHardwareInfo(IbricksMessage message, string hardwareInfo)
    {
        if (hardwareInfo.Contains("S36TX"))
        {
            logger.LogDebug("{ID}: S36TX found", message.MessageId);
            return new IbricksHardwareInfo(1, 0, 1, 0);
        }

        if (hardwareInfo.Contains("DIM_GL"))
        {
            logger.LogDebug("{ID}: DIM_GL found", message.MessageId);
            return new IbricksHardwareInfo(1, 0, 0, 1);
        }

        var r = 0;
        var s = 0;
        var h = 0;

        for (var i = 1; i < hardwareInfo.Length; i++)
        {
            switch (hardwareInfo[i])
            {
                case 'R':
                    r = hardwareInfo[i - 1];
                    break;
                case 'S':
                    s = hardwareInfo[i - 1];
                    break;
                case 'H':
                    h = hardwareInfo[i - 1];
                    break;
            }
        }

        var hInfo = new IbricksHardwareInfo(r, s, h, 0);
        logger.LogDebug("{ID}: Converted to: {Info}", message.MessageId, hInfo.ToString());

        return hInfo;
    }
}