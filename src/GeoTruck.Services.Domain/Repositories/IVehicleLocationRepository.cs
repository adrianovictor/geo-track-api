using GeoTruck.Services.Domain.Entities;

namespace GeoTruck.Services.Domain.Repositories;

public interface IVehicleLocationRepository
{
    Task SaveAsync(VehicleLocation vehicleLocation);
    Task DeleteAsync(VehicleLocation vehicleLocation);
}
