using GeoTruck.Services.Domain.Entities;
using GeoTruck.Services.Domain.Repositories;
using GeoTruck.Services.Infrastructure.Repositories.Common;
using Microsoft.Extensions.Logging;

namespace GeoTruck.Services.Infrastructure.Repositories;

public class VehicleLocationRepository(IRepository repository, ILogger<VehicleLocationRepository> logger) : IVehicleLocationRepository
{
    private readonly IRepository _repository = repository;
    private readonly ILogger<VehicleLocationRepository> _logger = logger;

    public Task DeleteAsync(VehicleLocation vehicleLocation)
    {
        throw new NotImplementedException();
    }

    public Task SaveAsync(VehicleLocation vehicleLocation)
    {
        throw new NotImplementedException();
    }
    
    public async Task<VehicleLocation?> GetByLocationAsync(long positionId, decimal latitude, decimal longitude, DateTime date, CancellationToken cancellationToken)
    {
        return await _repository.GetFirstOrDefaultAsync<VehicleLocation>(v =>
            v.PositionId == positionId &&
            v.Latitude == latitude &&
            v.Longitude == longitude &&
            v.Date == date,
            cancellationToken: cancellationToken);
    }
}
