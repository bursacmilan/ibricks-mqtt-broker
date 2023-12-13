using System.Net.Sockets;
using System.Text;
using ibricks_mqtt_broker.Model;
using ibricks_mqtt_broker.Services.Interface;
using Microsoft.Extensions.Logging;

namespace ibricks_mqtt_broker.Services.Cello.ToCello;

public class UdpSenderService(ILogger<UdpSenderService> logger) : IUdpSenderService, IDisposable
{
    private readonly UdpClient _udpClient = new();

    public async Task SendMessageAsync(string ipAddress, int port, IbricksMessage message)
    {
        logger.LogDebug("Sending message '{Message}' to {Ip}:{Port}", message, ipAddress, port);
        
        try
        {
            var messageBytes = Encoding.UTF8.GetBytes(message.ToString());
            await _udpClient.SendAsync(messageBytes, messageBytes.Length, ipAddress, port);
            logger.LogDebug("Message to {Ip}:{Port} sent", ipAddress, port);
        }
        catch (Exception e)
        {
            logger.LogDebug(e, "Could not send message '{Message}' to {Ip}:{Port}", message, ipAddress, port);
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _udpClient.Dispose();
    }
}