using GeoTruck.Services.Infrastructure.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GeoTruck.Services.CrossCutting.Extensions;

public static class DbContextServiceCollectionExtensions
{
    public static IServiceCollection AddDbContextExtensions(this IServiceCollection services, IConfiguration configuration)
    {
        var addContextDelegate = (IServiceProvider serviceProvider, string connectionStringName, DbContextOptionsBuilder builder) =>
        {
            var connectionString = configuration.GetConnectionString(connectionStringName);

            builder.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
            });
        };

        services.AddDbContext<ApplicationDbContext>((serviceProvider, builder) =>
            addContextDelegate(serviceProvider, "DefaultConnection", builder), ServiceLifetime.Scoped);

        return services;
    }
}
