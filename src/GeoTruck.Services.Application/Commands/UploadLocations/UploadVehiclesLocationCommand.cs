using GeoTruck.Services.Application.DTOs;
using MediatR;

namespace GeoTruck.Services.Application.Commands.UploadLocations;

/// <summary>Grava as posições do arquivo e devolve quantas foram gravadas.</summary>
public record UploadVehicleLocationsCommand(IEnumerable<VehicleRoutePositionDto> Locations) : IRequest<int>;
