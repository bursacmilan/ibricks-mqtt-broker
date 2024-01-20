using Microsoft.Extensions.DependencyInjection;

namespace ibricks_mqtt_broker.Services.Cello.ToCello.DeviceSateUpdater;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddStateUpdater(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<ClimateStateUpdater>();        
        serviceCollection.AddScoped<CoverStateUpdater>();        
        serviceCollection.AddScoped<DimmerStateUpdater>();        
        serviceCollection.AddScoped<RelayStateUpdater>();        
        
        return serviceCollection;
    }
}