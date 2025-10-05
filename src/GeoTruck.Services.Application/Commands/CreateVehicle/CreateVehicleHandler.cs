using GeoTruck.Services.Application.DTOs;
using GeoTruck.Services.Domain.Entities;
using GeoTruck.Services.Domain.Exceptions;
using GeoTruck.Services.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GeoTruck.Services.Application.Commands.CreateVehicle;

public class CreateVehicleHandler(IVehicleRepository vehicleRepository, ILogger<CreateVehicleHandler> logger) : IRequestHandler<CreateVehicleCommand, VehicleDto>
{
    private readonly IVehicleRepository _vehicleRepository = vehicleRepository;
    private readonly ILogger<CreateVehicleHandler> _logger = logger;

    public async Task<VehicleDto> Handle(CreateVehicleCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando criação de veículo com Renavam: {Renavam} e Placa: {Plate}", request.Renavam, request.Plate);
        var existingVehicle = await _vehicleRepository.GetByRenavamAsync(request.Renavam);

        if (existingVehicle != null)
        {
            _logger.LogWarning("Tentativa de criação de veículo falhou. Veículo com Renavam {Renavam} já existe.", request.Renavam);
            throw new VehicleAlreadyExistsException($"Veículo com Renavam {request.Renavam} já existe.");
        }

        var vehicle = Vehicle.Create(
            request.Renavam,
            request.Plate,
            request.Model,
            request.Brand,
            request.Year);

        _logger.LogInformation("Veículo criado com sucesso. Salvando no repositório.");
        await _vehicleRepository.SaveAsync(vehicle);

        return VehicleDto.Create(
            vehicle.Id,
            vehicle.UniqueId,
            vehicle.Plate.Value,
            vehicle.Model,
            vehicle.Brand,
            vehicle.Year,
            vehicle.Renavam);

    }
}
