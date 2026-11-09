using System.Net;
using System.Text.Json;
using GeoTruck.Services.Api.IntegrationTests.Infrastructure;

namespace GeoTruck.Services.Api.IntegrationTests.Vehicles;

public class UploadPositionsTests(GeoTrackApiFactory factory) : ApiTestBase(factory)
{
    private static string Positions(params (string Plate, long PositionId, decimal Lat, decimal Lng, string Date)[] positions) =>
        JsonSerializer.Serialize(positions.Select(p => new
        {
            plate = p.Plate,
            positionId = p.PositionId,
            latitude = p.Lat,
            longitude = p.Lng,
            date = p.Date,
            dateUTC = DateTime.Parse(p.Date).AddHours(3).ToString("s"),
        }));

    private async Task<JsonElement> ReadJsonAsync(HttpResponseMessage response) =>
        JsonDocument.Parse(await response.Content.ReadAsStringAsync(Ct)).RootElement;

    [Fact]
    public async Task AddsPositionsToTheVehicleWithThePlate()
    {
        var vehicle = await CreateVehicleAsync(plate: "ABC1234");

        using var response = await UploadAsync(Positions(
            ("ABC1234", 1001, -23.550520m, -46.633308m, "2026-11-01T08:00:00"),
            ("ABC1234", 1002, -23.551000m, -46.634000m, "2026-11-01T08:05:00")));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(2, (await ReadJsonAsync(response)).GetProperty("processedPositions").GetInt32());

        var stored = await GetVehicleAsync(vehicle.Id);
        Assert.Equal([1001L, 1002L], stored.Locations.Select(l => l.PositionId).Order());
        var first = stored.Locations.Single(l => l.PositionId == 1001);
        Assert.Equal(-23.550520m, first.Latitude);
        Assert.Equal(-46.633308m, first.Longitude);
        Assert.Equal(new DateTime(2026, 11, 1, 8, 0, 0), first.Date);
    }

    [Fact]
    public async Task SendingTheSameFileTwiceDoesNotDuplicatePositions()
    {
        var vehicle = await CreateVehicleAsync(plate: "ABC1234");
        var file = Positions(("ABC1234", 1001, -23.550520m, -46.633308m, "2026-11-01T08:00:00"));

        using (await UploadAsync(file)) { }
        using var second = await UploadAsync(file);

        Assert.Equal(HttpStatusCode.OK, second.StatusCode);
        Assert.Single((await GetVehicleAsync(vehicle.Id)).Locations);
    }

    [Fact]
    public async Task PositionsOfUnknownPlatesAreIgnored()
    {
        var vehicle = await CreateVehicleAsync(plate: "ABC1234");

        using var response = await UploadAsync(Positions(
            ("ABC1234", 1001, -23.550520m, -46.633308m, "2026-11-01T08:00:00"),
            ("ZZZ9999", 2001, -22.906847m, -43.172897m, "2026-11-01T08:00:00")));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Single((await GetVehicleAsync(vehicle.Id)).Locations);

        // BUG CONHECIDO: a resposta conta as posições enviadas, não as gravadas: diz 2 quando só 1 entrou.
        Assert.Equal(2, (await ReadJsonAsync(response)).GetProperty("processedPositions").GetInt32());
    }

    // Os erros de arquivo saem em ProblemDetails: a mensagem vai em "detail" (que o front mostra)
    // e também em errors.file.
    [Theory]
    [InlineData("{ isto não é json", "posicoes.json", "application/json", "Formato JSON inválido.")]
    [InlineData("[]", "posicoes.json", "application/json", "Arquivo vazio ou sem posições válidas.")]
    [InlineData("placa;lat;lng", "posicoes.csv", "text/csv", "Apenas arquivos JSON (.json) são permitidos.")]
    public async Task InvalidFileIsRejectedOnTheFileField(string content, string fileName, string contentType, string message)
    {
        using var response = await UploadAsync(content, fileName, contentType);

        var problem = await AssertProblemAsync(response, HttpStatusCode.BadRequest);
        Assert.Equal(message, problem.Detail);
        Assert.Equal([message], problem.Errors?["file"]);
    }

    [Fact]
    public async Task InvalidJsonDoesNotExposeTheDeserializerMessage()
    {
        using var response = await UploadAsync("{ isto não é json");

        var body = await ReadJsonAsync(response);
        Assert.False(body.TryGetProperty("details", out _));
        Assert.DoesNotContain("LineNumber", body.GetRawText());
    }
}
