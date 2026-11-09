using GeoTruck.Services.Application.DTOs;
using GeoTruck.Services.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GeoTruck.Services.Application.Queries.GetAllVehicles;

public class GetAllVehiclesHandler(IVehicleRepository repository, ILogger<GetAllVehiclesHandler> logger) : IRequestHandler<GetAllVehiclesCommand, GetAllVehiclesResponse>
{
    public async Task<GetAllVehiclesResponse> Handle(GetAllVehiclesCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando busca de veículos com filtros.");
        var pagedResult = await repository.GetVehiclesWithFiltersAsync(
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

        // Nenhum veículo com os filtros é resultado, não erro: responde 200 com a lista vazia.
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
