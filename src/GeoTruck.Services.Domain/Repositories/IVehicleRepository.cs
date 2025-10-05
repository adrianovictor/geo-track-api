using GeoTruck.Services.Domain.Entities;

namespace GeoTruck.Services.Domain.Repositories;

public interface IVehicleRepository
{
    IQueryable<Vehicle> GetAllAsync();
    Task SaveAsync(Vehicle vehicle);
    Task DeleteAsync(Vehicle vehicle);
}
