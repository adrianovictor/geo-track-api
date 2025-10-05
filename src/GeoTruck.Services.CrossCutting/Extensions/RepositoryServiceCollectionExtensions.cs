using System;
using GeoTruck.Services.Domain.Repositories;
using GeoTruck.Services.Infrastructure.Repositories;
using GeoTruck.Services.Infrastructure.Repositories.Common;
using Microsoft.Extensions.DependencyInjection;

namespace GeoTruck.Services.CrossCutting.Extensions;

public static class RepositoryServiceCollectionExtensions
{
    public static IServiceCollection AddRepositoryService(this IServiceCollection services)
    {
        services.AddScoped<IRepository, EntityRepository>();
        services.AddScoped<IVehicleRepository, VehicleRepository>();

        return services;
    }
}
