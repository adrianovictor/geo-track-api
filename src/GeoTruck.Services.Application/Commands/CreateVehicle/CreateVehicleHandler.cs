using GeoTruck.Services.Application.DTOs;
using GeoTruck.Services.Domain.Entities;
using GeoTruck.Services.Domain.Exceptions;
using GeoTruck.Services.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GeoTruck.Services.Application.Commands.CreateVehicle;

public class CreateVehicleHandler(IVehicleRepository vehicleRepository, ILogger<CreateVehicleHandler> logger) : IRequestHandler<CreateVehicleCommand, VehicleDto>
{
    public async Task<VehicleDto> Handle(CreateVehicleCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando criação de veículo com Renavam: {Renavam} e Placa: {Plate}", request.Renavam, request.Plate);
        // Cria antes de consultar o banco: a validação dos campos (400) vem antes do conflito (409),
        // e a placa já chega normalizada para a comparação.
        var vehicle = Vehicle.Create(
            request.Renavam,
            request.Plate,
            request.Model,
            request.Brand,
            request.Year);

        await vehicleRepository.ThrowIfInUseAsync(vehicle, cancellationToken: cancellationToken);

        logger.LogInformation("Veículo criado com sucesso. Salvando no repositório.");
        await vehicleRepository.SaveAsync(vehicle);

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
