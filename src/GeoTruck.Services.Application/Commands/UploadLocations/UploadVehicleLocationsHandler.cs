using GeoTruck.Services.Domain.Entities;
using GeoTruck.Services.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GeoTruck.Services.Application.Commands.UploadLocations;

public class UploadVehicleLocationsHandler(IVehicleRepository vehicleRepository, IVehicleLocationRepository vehicleLocationRepository, ILogger<UploadVehicleLocationsHandler> logger) : IRequestHandler<UploadVehicleLocationsCommand, Unit>
{
    private readonly IVehicleRepository _vehicleRepository = vehicleRepository;
    private readonly IVehicleLocationRepository _vehicleLocationRepository = vehicleLocationRepository;
    private readonly ILogger<UploadVehicleLocationsHandler> _logger = logger;

    public async Task<Unit> Handle(UploadVehicleLocationsCommand request, CancellationToken cancellationToken)
    {
        var locationsByPlate = request.Locations
            .GroupBy(l => l.Plate)
            .ToList();

        foreach (var group in locationsByPlate)
        {
            var plate = group.Key;
            var locations = group.ToList();

            var vehicle = await _vehicleRepository.GetByLicensePlateAsync(plate, cancellationToken);
            
            if (vehicle == null)
            {
                _logger.LogWarning(
                    "Veículo com placa {Plate} não encontrado. {Count} localizações ignoradas.",
                    plate,
                    locations.Count);
                continue;
            }

            foreach (var location in locations)
            {
                var existingLocation = await _vehicleLocationRepository.GetByLocationAsync(
                    location.PositionId,
                    location.Latitude,
                    location.Longitude,
                    location.Date);

                if (existingLocation == null)
                {
                    _logger.LogInformation(
                        "Adicionando localização {PositionId} para veículo {Plate}",
                        location.PositionId,
                        plate);
                        
                    vehicle.AddLocation(VehicleLocation.Create(
                        vehicle: vehicle,
                        latitude: location.Latitude,
                        longitude: location.Longitude,
                        positionId: location.PositionId,
                        date: location.Date,
                        dateUTC: location.DateUTC));
                }
                else
                {
                    _logger.LogInformation(
                        "Veículo com placa {Plate} já possui a localização {PositionId}. Ignorando.",
                        plate,
                        location.PositionId);
                }
            }

            _logger.LogInformation(
                "Adicionadas {Count} posições para veículo {Plate}",
                locations.Count,
                plate);

            await _vehicleRepository.SaveAsync(vehicle);
        }            

        return Unit.Value;
    }
}
