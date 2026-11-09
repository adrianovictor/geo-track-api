using GeoTruck.Services.Domain.Common;
using GeoTruck.Services.Domain.Entities;

namespace GeoTruck.Services.Domain.Repositories;

public interface IVehicleRepository
{
    Task<PagedResult<Vehicle>> GetVehiclesWithFiltersAsync(string? renavam, string? plate, string? model, string? brand, int? year, int offset, int limit, CancellationToken cancellationToken);
    Task<Vehicle?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Vehicle?> GetByLicensePlateAsync(string licensePlate, CancellationToken cancellationToken = default);
    Task<Vehicle?> GetByRenavamAsync(string renavam, CancellationToken cancellationToken = default);
    /// <summary>
    /// Indica se outro veículo já usa o RENAVAM. Conta também os excluídos,
    /// porque o índice único do banco vale para todos os registros.
    /// </summary>
    Task<bool> IsRenavamInUseAsync(string renavam, int? exceptVehicleId = null, CancellationToken cancellationToken = default);
    /// <summary>Indica se outro veículo já usa a placa. Conta também os excluídos, pelo mesmo motivo.</summary>
    Task<bool> IsPlateInUseAsync(string plate, int? exceptVehicleId = null, CancellationToken cancellationToken = default);
    Task SaveAsync(Vehicle vehicle);
    Task DeleteAsync(Vehicle vehicle);
}
