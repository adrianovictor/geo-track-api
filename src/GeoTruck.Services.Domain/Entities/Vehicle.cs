using GeoTruck.Services.Domain.Common;
using GeoTruck.Services.Domain.Enum;
using GeoTruck.Services.Domain.Exceptions;
using GeoTruck.Services.Domain.ValueOvject;

namespace GeoTruck.Services.Domain.Entities;

public class Vehicle : Statusable<Vehicle>
{
    private readonly List<VehicleLocation> _locations = [];

    public Guid UniqueId { get; protected set; }
    public string Renavam { get; protected set; }
    public BrazilianPlate Plate { get; protected set; }
    public string Model { get; protected set; }
    public string Brand { get; protected set; }
    public int Year { get; protected set; }
    public virtual IReadOnlyCollection<VehicleLocation> Locations => _locations;

    protected Vehicle()
    {
        UniqueId = Guid.NewGuid();
    }

    public Vehicle(string renavam, string plate, string model, string brand, int year, Status status = Status.Active) : this()
    {
        renavam.ThrowIfNullOrWhiteSpace(nameof(renavam));
        model.ThrowIfNullOrWhiteSpace(nameof(model));
        brand.ThrowIfNullOrWhiteSpace(nameof(brand));
        year.ThrowIfZeroOrNegative(nameof(year));
        year.ThrowIfValidYear(nameof(year));

        Renavam = renavam;
        Plate = new BrazilianPlate(plate);
        Model = model;
        Brand = brand;
        Year = year;
        Status = status;
    }

    public static Vehicle Create(string renavam, string plate, string model, string brand, int year, Status status = Status.Active)
    {
        return new Vehicle(renavam, plate, model, brand, year, status);
    }

    public void ChangeRenavam(string renavam)
    {
        renavam.ThrowIfNullOrWhiteSpace(nameof(renavam));
        Renavam = renavam;
    }

    public void ChangePlate(string plate)
    {
        Plate = new BrazilianPlate(plate);
    }

    public void ChangeModel(string model)
    {
        model.ThrowIfNullOrWhiteSpace(nameof(model));

        Model = model;
    }

    public void ChangeBrand(string brand)
    {
        brand.ThrowIfNullOrWhiteSpace(nameof(brand));

        Brand = brand;
    }

    public void ChangeYear(int year)
    {
        year.ThrowIfZeroOrNegative(nameof(year));
        year.ThrowIfValidYear(nameof(year));

        Year = year;
    }

    public void AddLocation(VehicleLocation location)
    {
        location.ThrowIfNull(nameof(location));

        if (_locations.Any(l => l.Equals(location)))
            throw new DuplicateVehicleLocationException();

        _locations.Add(location);
    }

    public void RemoveLocation(VehicleLocation location)
    {
        location.ThrowIfNull(nameof(location));
        
        var removed = _locations.Remove(location);
        
        if (!removed)
            throw new VehicleLocationNotFoundException();
    }
}
