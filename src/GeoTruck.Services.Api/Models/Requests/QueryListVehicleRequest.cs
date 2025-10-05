using System;

namespace GeoTruck.Services.Api.Models.Requests;

public class QueryListVehicleRequest
{
    public string? Renavam { get; set; }
    public string? Plate { get; set; }
    public string? Model { get; set; }
    public string? Brand { get; set; }
    public int? Year { get; set; }
    public int Limit { get; set; } = 10;
    public int Offset { get; set; } = 1;
}
