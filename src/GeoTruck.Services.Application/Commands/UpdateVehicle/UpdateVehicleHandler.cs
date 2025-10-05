using GeoTruck.Services.Application.DTOs;
using GeoTruck.Services.Domain.Exceptions;
using GeoTruck.Services.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GeoTruck.Services.Application.Commands.UpdateVehicle;

public class UpdateVehicleHandler(IVehicleRepository vehicleRepository, ILogger<UpdateVehicleHandler> logger) : IRequestHandler<UpdateVehicleCommand, VehicleDto>
{
    private readonly IVehicleRepository _vehicleRepository = vehicleRepository;
    private readonly ILogger<UpdateVehicleHandler> _logger = logger;

    public async Task<VehicleDto> Handle(UpdateVehicleCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando atualização de veículo com ID: {Id}", request.Id);
        var vehicle = await _vehicleRepository.GetByIdAsync(request.Id);

        if (vehicle is null)
        {
            _logger.LogWarning("Tentativa de atualização falhou. Veículo com ID {Id} não encontrado.", request.Id);
            throw new VehicleNotFoundException($"Veículo com ID {request.Id} não encontrado.");
        }

        _logger.LogInformation("Veículo encontrado. Atualizando informações.");
        vehicle.ChangeRenavam(request.Renavam);
        vehicle.ChangePlate(request.Plate);
        vehicle.ChangeModel(request.Model);
        vehicle.ChangeBrand(request.Brand);
        vehicle.ChangeYear(request.Year);

        _logger.LogInformation("Informações atualizadas. Salvando no repositório.");
        await _vehicleRepository.SaveAsync(vehicle);

        return VehicleDto.Create(
            vehicle.Id,
            vehicle.UniqueId,
            vehicle.Plate.Value,
            vehicle.Model,
            vehicle.Brand,
            vehicle.Year,
            vehicle.Renavam
        );
    }
}
