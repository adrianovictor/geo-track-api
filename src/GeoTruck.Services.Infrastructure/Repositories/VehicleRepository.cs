using GeoTruck.Services.Domain.Common;
using GeoTruck.Services.Domain.Entities;
using GeoTruck.Services.Domain.Repositories;
using GeoTruck.Services.Infrastructure.Extensions.Repositories;
using GeoTruck.Services.Infrastructure.Repositories.Common;
using Microsoft.EntityFrameworkCore;

namespace GeoTruck.Services.Infrastructure.Repositories;

public class VehicleRepository(IRepository repository) : IVehicleRepository
{
    private readonly IRepository _repository = repository;

    public Task DeleteAsync(Vehicle vehicle)
    {
        return _repository.DeleteAsync(vehicle);
    }

    public IQueryable<Vehicle> GetAllAsync()
    {
        return _repository.GetAllAsync<Vehicle>();
    }

    public async Task<Vehicle?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _repository.GetFirstOrDefaultAsync<Vehicle>(v => v.Id == id, cancellationToken);
    }

    public async Task<Vehicle?> GetByLicensePlateAsync(string licensePlate, CancellationToken cancellationToken = default)
    {
        return await _repository.GetFirstOrDefaultAsync<Vehicle>(v => v.Plate == licensePlate, cancellationToken);
    }

    public async Task<Vehicle?> GetByRenavamAsync(string renavam, CancellationToken cancellationToken = default)
    {
        return await _repository.GetFirstOrDefaultAsync<Vehicle>(v => v.Renavam == renavam, cancellationToken);
    }

    public async Task<PagedResult<Vehicle>> GetVehiclesWithFiltersAsync(string? renavam, string? plate, string? model, string? brand, int? year, int offset, int limit, CancellationToken cancellationToken)
    {        
        var query = _repository.GetQueryable<Vehicle>(); 
        var filteredQuery = query.ApplyFilters(renavam, plate, model, brand, year); 

        var totalRecords = await filteredQuery.CountAsync(cancellationToken); 

        var vehicles = await filteredQuery
            .Paginate(offset, limit)
            .ToListAsync(cancellationToken);

        return new PagedResult<Vehicle>(
            Items: vehicles,
            TotalRecords: totalRecords,
            CurrentPage: offset,
            PageSize: limit
        );
    }

    public Task SaveAsync(Vehicle vehicle)
    {
        return _repository.SaveAsync(vehicle);
    }
}
