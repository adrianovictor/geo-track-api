using GeoTruck.Services.Application.DTOs;
using GeoTruck.Services.Domain.Repositories;
using MediatR;
using GeoTruck.Services.Infrastructure.Extensions.Repositories;

namespace GeoTruck.Services.Application.Commands.UpdateVehicle;

public class UpdateVehicleHandler(IVehicleRepository vehicleRepository) : IRequestHandler<UpdateVehicleCommand, VehicleDto>
{
    private readonly IVehicleRepository _vehicleRepository = vehicleRepository;

    public async Task<VehicleDto> Handle(UpdateVehicleCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleRepository
            .GetAllAsync().GetByIdAsync(request.Id);

        if (vehicle is null)
        {
            throw new InvalidOperationException($"Veículo com ID {request.Id} não encontrado.");
        }

        vehicle.ChangeRenavam(request.Renavam);
        vehicle.ChangePlate(request.Plate);
        vehicle.ChangeModel(request.Model);
        vehicle.ChangeBrand(request.Brand);
        vehicle.ChangeYear(request.Year);

        await _vehicleRepository.SaveAsync(vehicle);

        return VehicleDto.Create(
            vehicle.Id,
            vehicle.UniqueId,
            vehicle.Plate,
            vehicle.Model,
            vehicle.Brand,
            vehicle.Year,
            vehicle.Renavam
        );
    }
}
