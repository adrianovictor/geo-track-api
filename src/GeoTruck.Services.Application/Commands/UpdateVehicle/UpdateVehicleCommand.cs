using GeoTruck.Services.Application.DTOs;
using MediatR;

namespace GeoTruck.Services.Application.Commands.UpdateVehicle;

public record UpdateVehicleCommand(int Id, string Renavam, string Plate, string Model, string Brand, int Year) : IRequest<VehicleDto>;
