namespace GeoTruck.Services.Application.DTOs;

public class VehicleLocationDto(
    int Id,
    int VehicleId,
    decimal Latitude,
    decimal Longitude,
    long PositionId,
    DateTime Date,
    DateTime DateUTC
)
{
    public int Id { get; set; } = Id;
    public int VehicleId { get; set; } = VehicleId;
    public decimal Latitude { get; set; } = Latitude;
    public decimal Longitude { get; set; } = Longitude;
    public long PositionId { get; set; } = PositionId;
    public DateTime Date { get; set; } = Date;
    public DateTime DateUTC { get; set; } = DateUTC;

    public static VehicleLocationDto Create(int id, int vehicleId, decimal latitude, decimal longitude, long positionId, DateTime date, DateTime dateUTC) =>
        new VehicleLocationDto(id, vehicleId, latitude, longitude, positionId, date, dateUTC);
}
