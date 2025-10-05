using System;
using MediatR;

namespace GeoTruck.Services.Application.Queries.GetAllVehicles;

public class GetAllVehiclesCommand(
    string? Renavam,
    string? Plate,
    string? Model,
    string? Brand,
    int? Year,
    int Limit,
    int Offset
) : IRequest<GetAllVehiclesResponse>
{
    public string? Renavam { get; set; } = Renavam;
    public string? Plate { get; set; } = Plate;
    public string? Model { get; set; } = Model;
    public string? Brand { get; set; } = Brand;
    public int? Year { get; set; } = Year;
    public int Limit { get; set; } = Limit;
    public int Offset { get; set; } = Offset;
}

