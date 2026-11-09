using GeoTruck.Services.Application.DTOs;
using MediatR;

namespace GeoTruck.Services.Application.Commands.CreateVehicle;

public record CreateVehicleCommand(string Renavam, string Plate, string Model, string Brand, int Year) : IRequest<VehicleDto>;
