using GeoTruck.Services.Domain.Exceptions;

namespace GeoTruck.Services.Domain.ValueOvject;

public class BrazilianPlate
{
    public string Value { get; }
    
    public BrazilianPlate(string value)
    {
        value.ThrowIfNullOrWhiteSpace(nameof(value));
        value.ThrowIfBrazilianVehiclePlateInvalid(nameof(value));
        Value = value.ToUpperInvariant();
    }
}