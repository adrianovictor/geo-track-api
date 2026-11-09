using GeoTruck.Services.Application.DTOs;
using GeoTruck.Services.Domain.Exceptions;
using GeoTruck.Services.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GeoTruck.Services.Application.Queries.GetById;

public class GetByIdHandler(IVehicleRepository vehicleRepository, ILogger<GetByIdHandler> logger) : IRequestHandler<GetByIdCommand, VehicleDto>
{
    public async Task<VehicleDto> Handle(GetByIdCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await vehicleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (vehicle == null)
        {
            logger.LogWarning("Veículo com Id {VehicleId} não encontrado.", request.Id);
            throw new VehicleNotFoundException();
        }

        logger.LogInformation("Veículo com Id {VehicleId} encontrado.", request.Id);

        return VehicleDto.Create(
            vehicle.Id,
            vehicle.UniqueId,
            vehicle.Plate.Value,
            vehicle.Model,
            vehicle.Brand,
            vehicle.Year,
            vehicle.Renavam,
            [.. vehicle.Locations.Select(loc => VehicleLocationDto.Create(
                loc.Id,
                vehicle.Id,
                loc.Latitude,
                loc.Longitude,
                loc.PositionId,
                loc.Date,
                loc.DateUTC))]
        );
    }
}
