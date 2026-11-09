using System.Linq.Expressions;
using GeoTruck.Services.Domain.Common;
using GeoTruck.Services.Domain.Entities;
using GeoTruck.Services.Domain.Repositories;
using GeoTruck.Services.Infrastructure.Extensions.Repositories;
using GeoTruck.Services.Infrastructure.Repositories.Common;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace GeoTruck.Services.Infrastructure.Repositories;

public class VehicleRepository(IRepository repository, ILogger<VehicleRepository> logger) : IVehicleRepository
{
    private readonly AsyncRetryPolicy _retryPolicy = CreateRetryPolicy(logger);

    private static AsyncRetryPolicy CreateRetryPolicy(ILogger logger) =>
        Policy
            .Handle<DbUpdateException>()
            .Or<SqlException>(ex =>
                ex.Number == -2 ||      // Timeout
                ex.Number == 1205 ||    // Deadlock
                ex.Number == 50000)     // Erro personalizado
            .WaitAndRetryAsync(3, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    logger.LogWarning(
                        exception,
                        "Tentativa {RetryCount} de 3 após {DelayMs}ms devido a {ExceptionType}: {Message}",
                        retryCount,
                        timeSpan.TotalMilliseconds,
                        exception.GetType().Name,
                        exception.Message);
                });

    public Task DeleteAsync(Vehicle vehicle)
    {
        return repository.DeleteAsync(vehicle);
    }

    public async Task<Vehicle?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _retryPolicy.ExecuteAsync(async () =>
            await repository.GetFirstOrDefaultAsync<Vehicle>(
                v => v.Id == id && v.Status != Domain.Enum.Status.Delete,
                includeProperties: [v => v.Locations],
                cancellationToken: cancellationToken));
    }

    public async Task<Vehicle?> GetByLicensePlateAsync(string licensePlate, CancellationToken cancellationToken = default)
    {
        return await _retryPolicy.ExecuteAsync(async () =>
        {
            logger.LogDebug("Buscando veículo com localizações por placa: {Plate}", licensePlate);

            var vehicle = await repository.GetQueryable<Vehicle>()
                .Include(v => v.Locations.OrderBy(l => l.Date))
                    .Where(v => v.Plate.Value == licensePlate && v.Status != Domain.Enum.Status.Delete)
                    .SingleOrDefaultAsync(cancellationToken);

            return vehicle;
        });
    }

    public async Task<Vehicle?> GetByRenavamAsync(string renavam, CancellationToken cancellationToken = default)
    {
        return await _retryPolicy.ExecuteAsync(async () =>
            await repository.GetFirstOrDefaultAsync<Vehicle>(
                v => v.Renavam == renavam && v.Status != Domain.Enum.Status.Delete,
                includeProperties: [v => v.Locations],
                cancellationToken: cancellationToken));
    }

    public async Task<PagedResult<Vehicle>> GetVehiclesWithFiltersAsync(string? renavam, string? plate, string? model, string? brand, int? year, int offset, int limit, CancellationToken cancellationToken)
    {
        // Valores fora do intervalo são corrigidos em vez de gerar erro. Sem isso, limit=0 dividia por zero
        // no cálculo da página.
        offset = Math.Max(0, offset);
        limit = Math.Max(1, limit);

        try
        {
            logger.LogInformation(
                "Buscando veículos com filtros- Renavam: {Renavam}, Plate: {Plate}, Offset: {Offset}, Limit: {Limit}",
                renavam, plate, offset, limit);

            var query = repository.GetQueryable<Vehicle>();
            var filteredQuery = query.ApplyFilters(renavam, plate, model, brand, year);

            var totalRecords = await filteredQuery.CountAsync(cancellationToken);

            var vehicles = await filteredQuery
                .Paginate(offset, limit)
                .ToListAsync(cancellationToken);

            logger.LogInformation("Retornados {Count} veículos de {Total}", vehicles.Count, totalRecords);
            return new PagedResult<Vehicle>(
                Items: vehicles,
                TotalRecords: totalRecords,
                CurrentPage: (offset / limit) + 1,
                PageSize: limit
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao obter veículos com filtros");
            throw;
        }
    }

    public Task<bool> IsRenavamInUseAsync(string renavam, int? exceptVehicleId = null, CancellationToken cancellationToken = default)
    {
        return _retryPolicy.ExecuteAsync(() =>
            repository.GetQueryable<Vehicle>()
                .AnyAsync(v => v.Renavam == renavam && v.Id != exceptVehicleId, cancellationToken));
    }

    public Task<bool> IsPlateInUseAsync(string plate, int? exceptVehicleId = null, CancellationToken cancellationToken = default)
    {
        return _retryPolicy.ExecuteAsync(() =>
            repository.GetQueryable<Vehicle>()
                .AnyAsync(v => v.Plate.Value == plate && v.Id != exceptVehicleId, cancellationToken));
    }

    public Task SaveAsync(Vehicle vehicle)
    {
        return repository.SaveAsync(vehicle);
    }
}
