using System;

namespace GeoTruck.Services.Domain.Exceptions;

public class NoVehiclesFoundException : DomainException
{
    public NoVehiclesFoundException() : base("Nenhum veículo encontrado.") { }
    public NoVehiclesFoundException(string message) : base(message) { }
}
