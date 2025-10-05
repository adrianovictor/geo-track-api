namespace GeoTruck.Services.Domain.Exceptions;

public class VehicleNotFoundException : DomainException
{
    public VehicleNotFoundException() : base("Veículo não encontrado.") { }
    public VehicleNotFoundException(string message) : base(message) { }
}
