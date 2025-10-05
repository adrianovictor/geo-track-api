using GeoTruck.Services.Domain.Exceptions;
using GeoTruck.Services.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GeoTruck.Services.Application.Commands.DeleteVehicle;

public class DeleteVehicleHandler(IVehicleRepository vehicleRepository, ILogger<DeleteVehicleHandler> logger) : IRequestHandler<DeleteVehicleCommand, bool>
{
    private readonly IVehicleRepository _vehicleRepository = vehicleRepository;
    private readonly ILogger<DeleteVehicleHandler> _logger = logger;

    public async Task<bool> Handle(DeleteVehicleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Iniciando remoção lógica de veículo com ID: {Id}", request.VehicleId);
            var vehicle = await _vehicleRepository.GetByIdAsync(request.VehicleId);

            if (vehicle is null)
            {
                _logger.LogWarning("Tentativa de remoção falhou. Veículo com ID {Id} não encontrado.", request.VehicleId);
                throw new VehicleNotFoundException($"Veículo com ID {request.VehicleId} não encontrado.");
            }

            vehicle.ChangeStatus(Domain.Enum.Status.Delete);

            _logger.LogInformation("Informações atualizadas. Salvando no repositório.");
            await _vehicleRepository.SaveAsync(vehicle);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar veículo com ID {VehicleId}", request.VehicleId);
            return false;
        }
    }
}
