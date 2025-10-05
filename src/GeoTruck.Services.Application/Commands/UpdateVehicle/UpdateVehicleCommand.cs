using GeoTruck.Services.Application.DTOs;
using MediatR;

namespace GeoTruck.Services.Application.Commands.UpdateVehicle;

public class UpdateVehicleCommand(int id, string renavam, string plate, string model, string brand, int year) : IRequest<VehicleDto>
{
    public int Id { get; set; } = id;
    public string Renavam { get; set; } = renavam;
    public string Plate { get; set; } = plate;
    public string Model { get; set; } = model;
    public string Brand { get; set; } = brand;
    public int Year { get; set; } = year;
}