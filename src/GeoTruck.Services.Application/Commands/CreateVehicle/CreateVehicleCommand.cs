using GeoTruck.Services.Application.DTOs;
using MediatR;

namespace GeoTruck.Services.Application.Commands.CreateVehicle;

public class CreateVehicleCommand(string renavam, string plate, string model, string brand, int year) : IRequest<VehicleDto>
{
    public string Renavam { get; set; } = renavam;
    public string Plate { get; set; } = plate;
    public string Model { get; set; } = model;
    public string Brand { get; set; } = brand;
    public int Year { get; set; } = year;
}
