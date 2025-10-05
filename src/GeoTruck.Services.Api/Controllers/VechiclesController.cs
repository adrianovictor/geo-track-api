using GeoTruck.Services.Api.Models.Requests;
using GeoTruck.Services.Application.Commands.CreateVehicle;
using GeoTruck.Services.Application.Commands.DeleteVehicle;
using GeoTruck.Services.Application.Commands.UpdateVehicle;
using GeoTruck.Services.Application.Queries.GetAllVehicles;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GeoTruck.Services.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VechiclesController(IMediator mediator, ILogger<VechiclesController> logger) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ILogger<VechiclesController> _logger = logger;

        [HttpGet]
        [Tags("Veículos")]
        [EndpointSummary("Lista Veículos")]
        [EndpointDescription("Retorna uma lista de todos os veículos cadastrados no sistema.")]
        public async Task<IActionResult> Get([FromQuery] QueryListVehicleRequest request)
        {
            var response = await _mediator.Send(new GetAllVehiclesCommand(
                request.Renavam,
                request.Plate,
                request.Model,
                request.Brand,
                request.Year,
                request.Limit,
                request.Offset
            ));

            return Ok(response);
        }

        [HttpPost]
        [Tags("Veículos")]
        public async Task<IActionResult> Post([FromBody] CreateVehicleRequest request)
        {
            var command = new CreateVehicleCommand(
                request.Renavam,
                request.Plate,
                request.Model,
                request.Brand,
                request.Year
            );

            var response = await _mediator.Send(command);
            return CreatedAtAction(nameof(Get), new { id = response.Id }, response);
        }


        [HttpPut("{id}")]
        [Tags("Veículos")]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateVehicleRequest request)
        {
            var command = new UpdateVehicleCommand(
                id,
                request.Renavam,
                request.Plate,
                request.Model,
                request.Brand,
                request.Year
            );

            var response = await _mediator.Send(command);
            
            return Ok(response);
        }

        [HttpDelete("{id}")]
        [Tags("Veículos")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Recebida solicitação para deletar veículo com ID: {Id}", id);
            var command = new DeleteVehicleCommand(id);
            var response = await _mediator.Send(command);

            return Ok(response);
        }
    }
}
