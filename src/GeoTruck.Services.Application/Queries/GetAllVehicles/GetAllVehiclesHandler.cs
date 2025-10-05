using GeoTruck.Services.Application.DTOs;
using GeoTruck.Services.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GeoTruck.Services.Application.Queries.GetAllVehicles;

public class GetAllVehiclesHandler(IVehicleRepository repository) : IRequestHandler<GetAllVehiclesCommand, GetAllVehiclesResponse>
{
    private readonly IVehicleRepository _repository = repository;
    public async Task<GetAllVehiclesResponse> Handle(GetAllVehiclesCommand request, CancellationToken cancellationToken)
    {
        var query = _repository.GetAllAsync();

        var filteredQuery = query.Where(vehicle =>
            (string.IsNullOrEmpty(request.Renavam) || vehicle.Renavam.Contains(request.Renavam)) &&
            (string.IsNullOrEmpty(request.Plate) || vehicle.Plate.Contains(request.Plate)) &&
            (string.IsNullOrEmpty(request.Model) || vehicle.Model.Contains(request.Model)) &&
            (string.IsNullOrEmpty(request.Brand) || vehicle.Brand.Contains(request.Brand)) &&
            (!request.Year.HasValue || vehicle.Year == request.Year)
        );
        var totalRecords = await filteredQuery.CountAsync(cancellationToken);
        var vehicles = await filteredQuery
            .Skip(request.Offset - 1)
            .Take(request.Limit)
            .ToListAsync();

        if (!vehicles.Any())
            throw new ApplicationException("Não há veículos cadastrados com os filtros informados.");

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
