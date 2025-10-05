namespace GeoTruck.Services.Application.DTOs;

public class VehicleDto
{
    public int Id { get; set; }
    public Guid UniqueId { get; set; }
    public string LicensePlate { get; set; }
    public string Model { get; set; }
    public string Brand { get; set; }
    public int Year { get; set; }
    public string Renavam { get; set; }

    public static VehicleDto Create(int id, Guid uniqueId, string licensePlate, string model, string brand, int year, string renavam) =>
        new VehicleDto
        {
            Id = id,
            UniqueId = uniqueId,
            LicensePlate = licensePlate,
            Model = model,
            Brand = brand,
            Year = year,
            Renavam = renavam
        };
}
