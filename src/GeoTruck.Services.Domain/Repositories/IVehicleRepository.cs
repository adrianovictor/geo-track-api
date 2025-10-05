using GeoTruck.Services.Domain.Common;
using GeoTruck.Services.Domain.Entities;

namespace GeoTruck.Services.Domain.Repositories;

public interface IVehicleRepository
{
    Task<PagedResult<Vehicle>> GetVehiclesWithFiltersAsync(string? renavam, string? plate, string? model, string? brand, int? year, int offset, int limit, CancellationToken cancellationToken);
    Task<Vehicle?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Vehicle?> GetByLicensePlateAsync(string licensePlate, CancellationToken cancellationToken = default);
    Task<Vehicle?> GetByRenavamAsync(string renavam, CancellationToken cancellationToken = default);
    Task SaveAsync(Vehicle vehicle);
    Task DeleteAsync(Vehicle vehicle);
}
