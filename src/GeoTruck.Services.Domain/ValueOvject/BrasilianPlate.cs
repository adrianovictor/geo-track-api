using GeoTruck.Services.Domain.Exceptions;

namespace GeoTruck.Services.Domain.ValueOvject;

public class BrazilianPlate
{
    public string Value { get; }
    
    public BrazilianPlate(string value)
    {
        value.ThrowIfNullOrWhiteSpace("plate");
        value = value.ToUpperInvariant();
        value.ThrowIfBrazilianVehiclePlateInvalid("plate");
        Value = value;
    }
}