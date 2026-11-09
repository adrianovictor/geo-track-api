using GeoTruck.Services.Domain.Entities;
using GeoTruck.Services.Domain.Exceptions;
using GeoTruck.Services.Domain.Repositories;

namespace GeoTruck.Services.Application.Commands;

internal static class VehicleRepositoryUniquenessExtensions
{
    /// <summary>
    /// Recusa com 409 o RENAVAM ou a placa que outro veículo já usa, antes que o índice único do banco
    /// recuse a gravação com um erro 500.
    /// </summary>
    public static async Task ThrowIfInUseAsync(
        this IVehicleRepository repository,
        Vehicle vehicle,
        int? exceptVehicleId = null,
        CancellationToken cancellationToken = default)
    {
        if (await repository.IsRenavamInUseAsync(vehicle.Renavam, exceptVehicleId, cancellationToken))
        {
            throw new VehicleAlreadyExistsException($"Já existe um veículo com o RENAVAM {vehicle.Renavam}.");
        }

        if (await repository.IsPlateInUseAsync(vehicle.Plate.Value, exceptVehicleId, cancellationToken))
        {
            throw new VehicleAlreadyExistsException($"Já existe um veículo com a placa {vehicle.Plate.Value}.");
        }
    }
}
