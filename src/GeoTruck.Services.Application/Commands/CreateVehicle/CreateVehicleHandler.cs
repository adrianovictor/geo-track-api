using GeoTruck.Services.Application.DTOs;
using GeoTruck.Services.Domain.Entities;
using GeoTruck.Services.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GeoTruck.Services.Application.Commands.CreateVehicle;

public class CreateVehicleHandler(IVehicleRepository vehicleRepository) : IRequestHandler<CreateVehicleCommand, VehicleDto>
{
    private readonly IVehicleRepository _vehicleRepository = vehicleRepository;

    public async Task<VehicleDto> Handle(CreateVehicleCommand request, CancellationToken cancellationToken)
    {
        var existingVehicle  = await _vehicleRepository
            .GetAllAsync()
            .SingleOrDefaultAsync(v => v.Renavam == request.Renavam, cancellationToken);

        if (existingVehicle != null)
        {
            throw new InvalidOperationException($"Veículo com Renavam {request.Renavam} já existe.");
        }

        var vehicle = Vehicle.Create(
            request.Renavam,
            request.Plate,
            request.Model,
            request.Brand,
            request.Year);

        await _vehicleRepository.SaveAsync(vehicle);

        return VehicleDto.Create(
            vehicle.Id,
            vehicle.UniqueId,
            vehicle.Plate,
            vehicle.Model,
            vehicle.Brand,
            vehicle.Year,
            vehicle.Renavam);

    }
}
