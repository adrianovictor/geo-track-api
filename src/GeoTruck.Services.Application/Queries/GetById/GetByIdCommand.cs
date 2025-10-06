using GeoTruck.Services.Application.DTOs;
using MediatR;

namespace GeoTruck.Services.Application.Queries.GetById;

public class GetByIdCommand(int Id) : IRequest<VehicleDto>
{
    public int Id { get; set; } = Id;
}
