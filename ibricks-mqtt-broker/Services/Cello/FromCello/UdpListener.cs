using System.Net.Sockets;
using System.Text;
using ibricks_mqtt_broker.Services.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ibricks_mqtt_broker.Services.Cello.FromCello;

public class UdpListener : IDisposable
{
    private readonly ILogger _logger;
    private readonly int _port;
    private readonly IServiceProvider _serviceProvider;

    private UdpClient? _udpClient;

    public UdpListener(ILogger logger, int port, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _port = port;
        _serviceProvider = serviceProvider;

        InitUdpClient();
    }

    public async Task StartListening()
    {
        if (_udpClient == null)
            return;

        try
        {
            _logger.LogDebug("Listening for incoming UDP messages on port {Port}", _port);

            while (true)
            {
                var receivedMessage = await _udpClient.ReceiveAsync();
                var ip = receivedMessage.RemoteEndPoint.Address.ToString();
                var content = Encoding.UTF8.GetString(receivedMessage.Buffer);

                try
                {
                    _ = Task.Run(() => HandleIncomingMessageAsync(content, ip));
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed to parse message from {Ip}: {Message}", ip, content);
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while listening to udp requests");
        }
        finally
        {
            _udpClient.Close();
        }
    }

    private async Task HandleIncomingMessageAsync(string content, string ip)
    {
        _logger.LogDebug("Received message from {Ip}: {Message}", ip, content);

        using var scope = _serviceProvider.CreateScope();
        var ibricksMessageInterpretor =
            scope.ServiceProvider.GetRequiredService<IIbricksMessageInterpretor>();
                    
        var ibricksMessageParserService =
            scope.ServiceProvider.GetRequiredService<IIbricksMessageParserService>();
                    
        var parsedMessage = ibricksMessageInterpretor.Interpret(content);
        await ibricksMessageParserService.ParseMessageAsync(parsedMessage);
    }

    private void InitUdpClient()
    {
        _udpClient = new UdpClient(_port);
    }

    public void Dispose()
    {
        _udpClient?.Dispose();
    }
}