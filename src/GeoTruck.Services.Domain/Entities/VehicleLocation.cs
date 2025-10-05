using GeoTruck.Services.Domain.Common;

namespace GeoTruck.Services.Domain.Entities;

public class VehicleLocation : Entity<VehicleLocation>
{
    public int VehicleId { get; protected set; }
    public virtual Vehicle Vehicle { get; protected set; }
    public double Latitude { get; protected set; }
    public double Longitude { get; protected set; }
    public long PositionId { get; protected set; }
    public DateTime Date { get; protected set; }
    public DateTime DateUTC { get; protected set; }

    protected VehicleLocation() { }

    public VehicleLocation(Vehicle vehicle, double latitude, double longitude, long positionId, DateTime date, DateTime dateUTC)
    {
        Vehicle = vehicle;
        VehicleId = vehicle.Id;
        Latitude = latitude;
        Longitude = longitude;
        PositionId = positionId;
        Date = date;
        DateUTC = dateUTC;
    }

    public static VehicleLocation Create(Vehicle vehicle, double latitude, double longitude, long positionId, DateTime date, DateTime dateUTC)
    {
        return new VehicleLocation(vehicle, latitude, longitude, positionId, date, dateUTC);
    }
}
