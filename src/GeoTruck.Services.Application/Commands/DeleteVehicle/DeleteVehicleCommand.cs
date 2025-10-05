using MediatR;

namespace GeoTruck.Services.Application.Commands.DeleteVehicle;

public class DeleteVehicleCommand(int VehicleId) : IRequest<bool>
{
    public int VehicleId { get; set; } = VehicleId; 

}
