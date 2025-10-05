using GeoTruck.Services.Application.DTOs;
using GeoTruck.Services.Domain.Repositories;
using GeoTruck.Services.Infrastructure.Extensions.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GeoTruck.Services.Application.Queries.GetAllVehicles;

public class GetAllVehiclesHandler(IVehicleRepository repository) : IRequestHandler<GetAllVehiclesCommand, GetAllVehiclesResponse>
{
    private readonly IVehicleRepository _repository = repository;
    public async Task<GetAllVehiclesResponse> Handle(GetAllVehiclesCommand request, CancellationToken cancellationToken)
    {
        var query = _repository.GetAllAsync();

        var filteredQuery = query.ApplyFilters(
            renavam: request.Renavam,
            plate: request.Plate,
            model: request.Model,
            brand: request.Brand,
            year: request.Year
        );

        var totalRecords = await filteredQuery.CountAsync(cancellationToken);

        var vehicles = await filteredQuery
            .Paginate(request.Offset, request.Limit)
            .ToListAsync(cancellationToken);

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
