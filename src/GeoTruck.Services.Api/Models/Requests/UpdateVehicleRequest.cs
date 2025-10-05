namespace GeoTruck.Services.Api.Models.Requests;

public class UpdateVehicleRequest
{
    public string Plate { get; set; }
    public string Model { get; set; }
    public string Brand { get; set; }
    public int Year { get; set; }
    public string Renavam { get; set; }
}
