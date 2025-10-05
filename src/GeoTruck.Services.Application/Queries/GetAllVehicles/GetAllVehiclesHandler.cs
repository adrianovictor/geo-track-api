using GeoTruck.Services.Application.DTOs;
using GeoTruck.Services.Domain.Exceptions;
using GeoTruck.Services.Domain.Repositories;
using MediatR;

namespace GeoTruck.Services.Application.Queries.GetAllVehicles;

public class GetAllVehiclesHandler(IVehicleRepository repository) : IRequestHandler<GetAllVehiclesCommand, GetAllVehiclesResponse>
{
    private readonly IVehicleRepository _repository = repository;
    public async Task<GetAllVehiclesResponse> Handle(GetAllVehiclesCommand request, CancellationToken cancellationToken)
    {
        var pagedResult = await _repository.GetVehiclesWithFiltersAsync(
            renavam: request.Renavam,
            plate: request.Plate,
            model: request.Model,
            brand: request.Brand,
            year: request.Year,
            offset: request.Offset,
            limit: request.Limit,
            cancellationToken);

        var vehicles = pagedResult.Items;
        var totalRecords = pagedResult.TotalRecords;

        if (vehicles.Count() == 0)
        {
            throw new VehicleNotFoundException("Nenhum veículo encontrado.");
        }

        return new GetAllVehiclesResponse
        {
            Vehicles = vehicles.Select(v => VehicleDto.Create(
                v.Id,
                v.UniqueId,
                v.Plate,
                v.Model,
                v.Brand,
                v.Year,
                v.Renavam)),
            TotalRecords = totalRecords,
            CurrentPage = request.Offset,
            PageItens = request.Limit
        };
    }
}
