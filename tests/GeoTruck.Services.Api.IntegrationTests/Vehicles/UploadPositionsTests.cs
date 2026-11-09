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

        // A resposta conta as posições enviadas, não as gravadas: diz 2 quando só 1 entrou.
        Assert.Equal(2, (await ReadJsonAsync(response)).GetProperty("processedPositions").GetInt32());
    }

    [Fact]
    public async Task InvalidJsonIsRejected()
    {
        using var response = await UploadAsync("{ isto não é json");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await ReadJsonAsync(response);
        Assert.Equal("Formato JSON inválido.", body.GetProperty("error").GetString());
        // A resposta também devolve a mensagem interna do desserializador no campo "details".
        Assert.True(body.TryGetProperty("details", out _));
    }

    [Fact]
    public async Task EmptyListIsRejected()
    {
        using var response = await UploadAsync("[]");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Arquivo vazio ou sem posições válidas.", (await ReadJsonAsync(response)).GetProperty("error").GetString());
    }

    [Fact]
    public async Task NonJsonFileIsRejected()
    {
        using var response = await UploadAsync("placa;lat;lng", fileName: "posicoes.csv", contentType: "text/csv");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Apenas arquivos JSON (.json) são permitidos.", (await ReadJsonAsync(response)).GetProperty("error").GetString());
    }
}
