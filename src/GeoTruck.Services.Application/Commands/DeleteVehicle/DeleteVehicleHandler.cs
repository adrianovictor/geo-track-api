using GeoTruck.Services.Domain.Exceptions;
using GeoTruck.Services.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GeoTruck.Services.Application.Commands.DeleteVehicle;

public class DeleteVehicleHandler(IVehicleRepository vehicleRepository, ILogger<DeleteVehicleHandler> logger) : IRequestHandler<DeleteVehicleCommand>
{
    public async Task Handle(DeleteVehicleCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando remoção lógica de veículo com ID: {Id}", request.VehicleId);

        // Veículo já excluído também não é encontrado: excluir de novo responde 404.
        var vehicle = await vehicleRepository.GetByIdAsync(request.VehicleId, cancellationToken)
            ?? throw new VehicleNotFoundException($"Veículo com ID {request.VehicleId} não encontrado.");

        vehicle.ChangeStatus(Domain.Enum.Status.Delete);

        logger.LogInformation("Informações atualizadas. Salvando no repositório.");
        await vehicleRepository.SaveAsync(vehicle);
    }
}
