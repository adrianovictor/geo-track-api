using MediatR;

namespace GeoTruck.Services.Application.Commands.DeleteVehicle;

public class DeleteVehicleCommand(int VehicleId) : IRequest
{
    public int VehicleId { get; set; } = VehicleId;
}
