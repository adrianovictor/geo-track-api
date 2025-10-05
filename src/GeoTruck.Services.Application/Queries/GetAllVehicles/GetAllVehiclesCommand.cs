using System;
using MediatR;

namespace GeoTruck.Services.Application.Queries.GetAllVehicles;

public record GetAllVehiclesCommand(
    string? Renavam,
    string? Plate,
    string? Model,
    string? Brand,
    int? Year,
    int Limit,
    int Offset
) : IRequest<GetAllVehiclesResponse>;

