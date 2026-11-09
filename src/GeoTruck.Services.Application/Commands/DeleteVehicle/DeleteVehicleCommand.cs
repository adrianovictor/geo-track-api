using MediatR;

namespace GeoTruck.Services.Application.Commands.DeleteVehicle;

public record DeleteVehicleCommand(int VehicleId) : IRequest;
