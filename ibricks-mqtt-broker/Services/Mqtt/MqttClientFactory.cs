using ibricks_mqtt_broker.Infrastructure;
using ibricks_mqtt_broker.Services.Interface;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;

namespace ibricks_mqtt_broker.Services.Mqtt;

public class MqttClientFactory(
    ILogger<MqttClientFactory> logger,
    IOptionsMonitor<GlobalSettings> optionsMonitorGlobalSettings) : IDisposable, IMqttClientFactory
{
    private IMqttClient? _mqttClient;
    private bool _disposed;
    private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

    public event Func<Task>? OnReconnect;

    public async Task<IMqttClient> GetMqttClientAsync()
    {
        try
        {
            await _semaphoreSlim.WaitAsync();
            return await GetMqttClientInternalAsync();
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    private async Task<IMqttClient> GetMqttClientInternalAsync()
    {
        if (_mqttClient != null)
            return _mqttClient;

        var mqttFactory = new MqttFactory();
        _mqttClient = mqttFactory.CreateMqttClient();

        if (optionsMonitorGlobalSettings.CurrentValue.MqttSettings?.Host == null)
        {
            logger.LogError("MQTT not configured");
            throw new Exception("MQTT not configured");
        }

        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(optionsMonitorGlobalSettings.CurrentValue.MqttSettings.Host);

        if (optionsMonitorGlobalSettings.CurrentValue.MqttSettings.Username != null &&
            optionsMonitorGlobalSettings.CurrentValue.MqttSettings.Password != null)
        {
            mqttClientOptions.WithCredentials(optionsMonitorGlobalSettings.CurrentValue.MqttSettings.Username,
                optionsMonitorGlobalSettings.CurrentValue.MqttSettings.Password);
        }

        var builderOptions = mqttClientOptions.Build();
        await _mqttClient.ConnectAsync(builderOptions);

        _ = Task.Run(
            async () =>
            {
                while (!_disposed)
                {
                    try
                    {
                        if (await _mqttClient.TryPingAsync())
                            continue;

                        await _mqttClient.ConnectAsync(builderOptions);

                        if (OnReconnect != null)
                            await OnReconnect.Invoke();
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Cant connect mqttClient");
                    }
                    finally
                    {
                        await Task.Delay(TimeSpan.FromSeconds(5));
                    }
                }
            });

        return _mqttClient;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _disposed = true;
        _mqttClient?.Dispose();
    }
}