using GeoTruck.Services.Application.DTOs;

namespace GeoTruck.Services.Application.Queries.GetAllVehicles;

public class GetAllVehiclesResponse : Pagination
{
    public IEnumerable<VehicleDto> Vehicles { get; set; } = [];
}

public class Pagination
{
    public int TotalRecords { get; set; }
    public int CurrentPage { get; set; }
    public int PageItens { get; set; }
}
