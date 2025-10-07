using System.Text.Json;
using GeoTruck.Services.Api.Models.Requests;
using GeoTruck.Services.Application.Commands.CreateVehicle;
using GeoTruck.Services.Application.Commands.DeleteVehicle;
using GeoTruck.Services.Application.Commands.UpdateVehicle;
using GeoTruck.Services.Application.Commands.UploadLocations;
using GeoTruck.Services.Application.DTOs;
using GeoTruck.Services.Application.Queries.GetAllVehicles;
using GeoTruck.Services.Application.Queries.GetById;
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

        #region GET
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

        [HttpGet("{id}")]
        [Tags("Veículos")]
        [EndpointSummary("Detalhes do Veículo")]
        [EndpointDescription("Retorna os detalhes de um veículo específico pelo seu ID, incluindo suas localizações.")]
        public async Task<IActionResult> Get(int id)
        {
            var response = await _mediator.Send(new GetByIdCommand(id));
            return Ok(response);
        }
        #endregion

        #region POST
        [HttpPost]
        [Tags("Veículos")]
        [EndpointSummary("Criação de Veículo")]
        [EndpointDescription("Cria um novo veículo no sistema.")]
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

        [HttpPost("upload-vehicles-route-positions")]
        [Tags("Veículos")]
        [EndpointSummary("Upload de posições de rota do veículo.")]
        [EndpointDescription("Faz upload de um arquivo JSON contendo posições de rota. Máximo: 10MB, 10.000 posições.")]
        [RequestSizeLimit(10 * 1024 * 1024)]
        public async Task<IActionResult> UploadVehiclesRoutePositions([FromForm] IFormFile file)
        {
            try
            {
                var validationResult = ValidateUploadFile(file);
                if (validationResult != null)
                    return validationResult;

                List<VehicleRoutePositionDto>? positions;
                try
                {
                    using var stream = file.OpenReadStream();
                    positions = await JsonSerializer.DeserializeAsync<List<VehicleRoutePositionDto>>(
                        stream,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                            MaxDepth = 64 // Prevenir JSON muito aninhado
                        });
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning(ex, "JSON inválido no upload de posições");
                    return BadRequest(new
                    {
                        error = "Formato JSON inválido.",
                        details = ex.Message
                    });
                }   
                
                if (positions == null || positions.Count == 0)
                    return BadRequest(new { error = "Arquivo vazio ou sem posições válidas." });

                const int maxPositions = 10000;
                if (positions.Count > maxPositions)
                {
                    return BadRequest(new 
                    { 
                        error = $"Arquivo contém {positions.Count} posições. Máximo permitido: {maxPositions}." 
                    });
                }

                _logger.LogInformation(
                    "Processando upload de {Count} posições de rota",
                    positions.Count);

                var response = await _mediator.Send(new UploadVehicleLocationsCommand(positions));

                return Ok(new
                {
                    message = "Upload processado com sucesso.",
                    processedPositions = positions.Count,
                    timestamp = DateTime.UtcNow
                });                                 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar o arquivo enviado.");
                return StatusCode(500, new
                {
                    error = "Erro interno ao processar upload. Contate o suporte."
                });
            }
        }
        #endregion

        #region PUT
        [HttpPut("{id}")]
        [Tags("Veículos")]
        [EndpointSummary("Atualização de Veículo")]
        [EndpointDescription("Atualiza os detalhes de um veículo existente.")]
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
        #endregion

        #region DELETE
        [HttpDelete("{id}")]
        [Tags("Veículos")]
        [EndpointSummary("Deleção de Veículo")]
        [EndpointDescription("Remove um veículo do sistema.")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Recebida solicitação para deletar veículo com ID: {Id}", id);
            var command = new DeleteVehicleCommand(id);
            var response = await _mediator.Send(command);

            return Ok(response);
        }
        #endregion

        #region Private Members
        private IActionResult? ValidateUploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { error = "Arquivo não enviado." });

            const long maxFileSize = 10 * 1024 * 1024; // 10MB
            if (file.Length > maxFileSize)
            {
                return BadRequest(new 
                { 
                    error = $"Arquivo muito grande ({file.Length / 1024 / 1024}MB). Máximo permitido: 10MB." 
                });
            }

            var allowedContentTypes = new[] { "application/json", "text/json" };
            if (!allowedContentTypes.Contains(file.ContentType) && 
                !file.FileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { error = "Apenas arquivos JSON (.json) são permitidos." });
            }

            return null; // Validação OK
        }        
        #endregion
    }
}
