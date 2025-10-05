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

public class VehicleRepository : IVehicleRepository
{
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly IRepository _repository;
    private readonly ILogger<VehicleRepository> _logger;

    public VehicleRepository(IRepository repository, ILogger<VehicleRepository> logger)
    {
        _repository = repository;
        _logger = logger;

        _retryPolicy = Policy
            .Handle<DbUpdateException>()
            .Or<SqlException>(ex => 
                ex.Number == -2 ||      // Timeout
                ex.Number == 1205 ||    // Deadlock
                ex.Number == 50000)     // Erro personalizado
            .WaitAndRetryAsync(3, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning(
                        exception,
                        "Tentativa {RetryCount} de 3 após {DelayMs}ms devido a {ExceptionType}: {Message}",
                        retryCount, 
                        timeSpan.TotalMilliseconds,
                        exception.GetType().Name,
                        exception.Message);
                });
    }

    public Task DeleteAsync(Vehicle vehicle)
    {
        return _repository.DeleteAsync(vehicle);
    }

    public async Task<Vehicle?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _retryPolicy.ExecuteAsync(async () =>
            await _repository.GetFirstOrDefaultAsync<Vehicle>(
                v => v.Id == id && v.Status != Domain.Enum.Status.Delete,
                includeProperties: [v => v.Locations],
                cancellationToken: cancellationToken));
    }

    public async Task<Vehicle?> GetByLicensePlateAsync(string licensePlate, CancellationToken cancellationToken = default)
    {
        return await _retryPolicy.ExecuteAsync(async () =>
            await _repository.GetFirstOrDefaultAsync<Vehicle>(
                v => v.Plate.Value == licensePlate && v.Status != Domain.Enum.Status.Delete,
                includeProperties: [v => v.Locations],
                cancellationToken: cancellationToken));
    }

    public async Task<Vehicle?> GetByRenavamAsync(string renavam, CancellationToken cancellationToken = default)
    {
        return await _retryPolicy.ExecuteAsync(async () =>
            await _repository.GetFirstOrDefaultAsync<Vehicle>(
                v => v.Renavam == renavam && v.Status != Domain.Enum.Status.Delete,
                includeProperties: [v => v.Locations],
                cancellationToken: cancellationToken));
    }

    public async Task<PagedResult<Vehicle>> GetVehiclesWithFiltersAsync(string? renavam, string? plate, string? model, string? brand, int? year, int offset, int limit, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Buscando veículos com filtros- Renavam: {Renavam}, Plate: {Plate}, Offset: {Offset}, Limit: {Limit}",
                renavam, plate, offset, limit);

            var query = _repository.GetQueryable<Vehicle>(); 
            var filteredQuery = query.ApplyFilters(renavam, plate, model, brand, year); 

            var totalRecords = await filteredQuery.CountAsync(cancellationToken); 

            var vehicles = await filteredQuery
                .Paginate(offset, limit)
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Retornados {Count} veículos de {Total}", vehicles.Count, totalRecords);
            return new PagedResult<Vehicle>(
                Items: vehicles,
                TotalRecords: totalRecords,
                CurrentPage: (offset / limit) + 1,
                PageSize: limit
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter veículos com filtros");
            throw;            
        }
    }

    public Task SaveAsync(Vehicle vehicle)
    {
        return _repository.SaveAsync(vehicle);
    }
}
