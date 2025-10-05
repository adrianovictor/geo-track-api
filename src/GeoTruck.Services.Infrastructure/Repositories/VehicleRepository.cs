using GeoTruck.Services.Domain.Entities;
using GeoTruck.Services.Domain.Repositories;
using GeoTruck.Services.Infrastructure.Repositories.Common;

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

    public Task SaveAsync(Vehicle vehicle)
    {
        return _repository.SaveAsync(vehicle);
    }
}
