using System;

namespace GeoTruck.Services.Domain.Exceptions;

public class VehicleAlreadyExistsException : DomainException
{
    public VehicleAlreadyExistsException() : base("Veículo já existe.") { }
    public VehicleAlreadyExistsException(string message) : base(message) { }
}
