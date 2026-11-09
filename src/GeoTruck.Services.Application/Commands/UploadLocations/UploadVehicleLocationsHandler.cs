using GeoTruck.Services.Domain.Entities;
using GeoTruck.Services.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GeoTruck.Services.Application.Commands.UploadLocations;

public class UploadVehicleLocationsHandler(IVehicleRepository vehicleRepository, IVehicleLocationRepository vehicleLocationRepository, ILogger<UploadVehicleLocationsHandler> logger) : IRequestHandler<UploadVehicleLocationsCommand, int>
{
    public async Task<int> Handle(UploadVehicleLocationsCommand request, CancellationToken cancellationToken)
    {
        var locationsByPlate = request.Locations
            .GroupBy(l => l.Plate)
            .ToList();

        // Conta só as posições gravadas: placa desconhecida e posição repetida ficam de fora.
        var added = 0;

        foreach (var group in locationsByPlate)
        {
            var plate = group.Key;
            var locations = group.ToList();
            var addedForVehicle = 0;

            var vehicle = await vehicleRepository.GetByLicensePlateAsync(plate, cancellationToken);
            
            if (vehicle == null)
            {
                logger.LogWarning(
                    "Veículo com placa {Plate} não encontrado. {Count} localizações ignoradas.",
                    plate,
                    locations.Count);
                continue;
            }

            foreach (var location in locations)
            {
                var existingLocation = await vehicleLocationRepository.GetByLocationAsync(
                    location.PositionId,
                    location.Latitude,
                    location.Longitude,
                    location.Date);

                if (existingLocation == null)
                {
                    logger.LogInformation(
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
                    addedForVehicle++;
                }
                else
                {
                    logger.LogInformation(
                        "Veículo com placa {Plate} já possui a localização {PositionId}. Ignorando.",
                        plate,
                        location.PositionId);
                }
            }

            logger.LogInformation(
                "Adicionadas {Count} de {Received} posições para veículo {Plate}",
                addedForVehicle,
                locations.Count,
                plate);

            await vehicleRepository.SaveAsync(vehicle);
            added += addedForVehicle;
        }            

        return added;
    }
}
