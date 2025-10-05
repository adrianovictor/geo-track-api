namespace GeoTruck.Services.Domain.Exceptions;

public class VehicleLocationNotFoundException : DomainException
{
    public VehicleLocationNotFoundException() : base("A localização não foi encontrada para este veículo.") { }
    public VehicleLocationNotFoundException(string message) : base(message) { }
}
