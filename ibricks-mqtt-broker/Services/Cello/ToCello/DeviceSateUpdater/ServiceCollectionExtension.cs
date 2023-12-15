using Microsoft.Extensions.DependencyInjection;

namespace ibricks_mqtt_broker.Services.Cello.ToCello.DeviceSateUpdater;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddStateUpdater(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<ClimateStateUpdater>();        
        serviceCollection.AddTransient<CoverStateUpdater>();        
        serviceCollection.AddTransient<DimmerStateUpdater>();        
        serviceCollection.AddTransient<RelayStateUpdater>();        
        
        return serviceCollection;
    }
}