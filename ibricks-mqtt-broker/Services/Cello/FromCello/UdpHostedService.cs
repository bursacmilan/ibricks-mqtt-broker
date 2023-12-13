using ibricks_mqtt_broker.Services.Interface;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ibricks_mqtt_broker.Services.Cello.FromCello;

public class UdpHostedService(
    ILogger<UdpHostedService> logger,
    IIbricksMessageParserService ibricksMessageParserService,
    IIbricksMessageInterpretor ibricksMessageInterpretor) : IHostedService
{
    private UdpListener? _udpListener;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(() => Run(cancellationToken), cancellationToken);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _udpListener?.Dispose();
        return Task.CompletedTask;
    }

    private async Task Run(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            _udpListener = new UdpListener(logger, NetworkDefaults.UdpPort, ibricksMessageParserService,
                ibricksMessageInterpretor);
            await _udpListener.StartListening();
        }
    }
}