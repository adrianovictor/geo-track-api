using Core.Common.Domain.Exceptions;

namespace GeoTruck.Services.Domain.Exceptions;

/// <summary>Já existe um veículo com o mesmo RENAVAM ou a mesma placa. A API responde 409.</summary>
public class VehicleAlreadyExistsException : ConflictException
{
    public VehicleAlreadyExistsException() : base("Veículo já existe.") { }
    public VehicleAlreadyExistsException(string message) : base(message) { }
}
