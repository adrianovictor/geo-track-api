namespace GeoTruck.Services.Application.DTOs;

public record VehicleLocationDto(
    int Id,
    int VehicleId,
    decimal Latitude,
    decimal Longitude,
    long PositionId,
    DateTime Date,
    DateTime DateUTC
)
{
    public static VehicleLocationDto Create(int id, int vehicleId, decimal latitude, decimal longitude, long positionId, DateTime date, DateTime dateUTC) =>
        new(id, vehicleId, latitude, longitude, positionId, date, dateUTC);
}
