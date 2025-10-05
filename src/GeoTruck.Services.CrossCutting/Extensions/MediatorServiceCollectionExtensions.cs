using Microsoft.Extensions.DependencyInjection;

namespace GeoTruck.Services.CrossCutting.Extensions;

public static class MediatorServiceCollectionExtensions
{
    public static IServiceCollection AddMediatorExtensions(this IServiceCollection services)
    {
        var assembly = AppDomain.CurrentDomain.Load("GeoTruck.Services.Application");
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        return services;
    }
}
