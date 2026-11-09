using Core.Common.Domain.Exceptions;

namespace GeoTruck.Services.Domain.Exceptions;

/// <summary>Veículo inexistente ou excluído. A API responde 404.</summary>
public class VehicleNotFoundException : NotFoundException
{
    public VehicleNotFoundException() : base("Veículo não encontrado.") { }
    public VehicleNotFoundException(string message) : base(message) { }
}
