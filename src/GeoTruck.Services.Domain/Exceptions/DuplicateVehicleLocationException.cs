namespace GeoTruck.Services.Domain.Exceptions;

public class DuplicateVehicleLocationException : DomainException
{
    public DuplicateVehicleLocationException() : base("A localização já foi adicionada a este veículo.") { }
    public DuplicateVehicleLocationException(string message) : base(message) { }
}
