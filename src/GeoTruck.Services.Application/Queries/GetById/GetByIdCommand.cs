using GeoTruck.Services.Application.DTOs;
using MediatR;

namespace GeoTruck.Services.Application.Queries.GetById;

public record GetByIdCommand(int Id) : IRequest<VehicleDto>;
