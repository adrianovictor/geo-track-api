using GeoTruck.Services.Application.DTOs;
using GeoTruck.Services.Domain.Exceptions;
using GeoTruck.Services.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GeoTruck.Services.Application.Queries.GetAllVehicles;

public class GetAllVehiclesHandler(IVehicleRepository repository, ILogger<GetAllVehiclesHandler> logger) : IRequestHandler<GetAllVehiclesCommand, GetAllVehiclesResponse>
{
    private readonly IVehicleRepository _repository = repository;
    private readonly ILogger<GetAllVehiclesHandler> _logger = logger;

    public async Task<GetAllVehiclesResponse> Handle(GetAllVehiclesCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando busca de veículos com filtros.");
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

        if (!vehicles.Any())
        {
            _logger.LogWarning("Nenhum veículo encontrado com os filtros fornecidos.");
            throw new NoVehiclesFoundException();
        }

        return new GetAllVehiclesResponse
        {
            Vehicles = vehicles.Select(v => VehicleDto.Create(
                v.Id,
                v.UniqueId,
                v.Plate.Value,
                v.Model,
                v.Brand,
                v.Year,
                v.Renavam)),
            TotalRecords = totalRecords,
            CurrentPage = pagedResult.CurrentPage,
            PageItens = pagedResult.PageSize
        };
    }
}
