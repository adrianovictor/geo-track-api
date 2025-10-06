using GeoTruck.Services.Application.DTOs;
using MediatR;

namespace GeoTruck.Services.Application.Commands.UploadLocations;

public record UploadVehicleLocationsCommand(IEnumerable<VehicleRoutePositionDto> Locations) : IRequest<Unit>;
