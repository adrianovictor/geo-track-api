using GeoTruck.Services.Domain.Entities;

namespace GeoTruck.Services.Domain.Repositories;

public interface IVehicleLocationRepository
{
    Task<VehicleLocation?> GetByLocationAsync(long positionId, decimal latitude, decimal longitude, DateTime date, CancellationToken cancellationToken = default);
    Task SaveAsync(VehicleLocation vehicleLocation);
    Task DeleteAsync(VehicleLocation vehicleLocation);
}
