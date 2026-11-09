using System.Net;
using System.Net.Http.Json;
using GeoTruck.Services.Api.IntegrationTests.Infrastructure;

namespace GeoTruck.Services.Api.IntegrationTests.Vehicles;

public class CreateVehicleTests(GeoTrackApiFactory factory) : ApiTestBase(factory)
{
    [Fact]
    public async Task ValidVehicleReturns201WithLocationOfTheNewResource()
    {
        using var response = await PostVehicleAsync(NewVehicle(plate: "ABC1234", renavam: "12345678901"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<VehicleResponse>(Ct);
        Assert.NotNull(created);
        Assert.True(created.Id > 0);
        Assert.NotEqual(Guid.Empty, created.UniqueId);
        Assert.Equal("ABC1234", created.Plate);
        Assert.Equal("12345678901", created.Renavam);
        Assert.EndsWith($"{VehiclesUrl}/{created.Id}", response.Headers.Location?.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CreatedVehicleCanBeReadBack()
    {
        var created = await CreateVehicleAsync(plate: "XYZ9876", model: "Actros", brand: "Mercedes-Benz", year: 2020);

        var stored = await GetVehicleAsync(created.Id);

        Assert.Equal("XYZ9876", stored.Plate);
        Assert.Equal("Actros", stored.Model);
        Assert.Equal("Mercedes-Benz", stored.Brand);
        Assert.Equal(2020, stored.Year);
        Assert.Empty(stored.Locations);
    }

    [Theory]
    [InlineData("abc1234", "ABC1234")]
    [InlineData("ABC1D23", "ABC1D23")]   // padrão Mercosul
    [InlineData("abc-1234", "ABC-1234")] // o hífen é aceito e gravado como veio
    public async Task AcceptsOldAndMercosulPlatesAndStoresThemUppercase(string plate, string stored)
    {
        var created = await CreateVehicleAsync(plate: plate);

        Assert.Equal(stored, created.Plate);
    }

    [Fact]
    public async Task AcceptsCurrentYear()
    {
        var created = await CreateVehicleAsync(year: DateTime.UtcNow.Year);

        Assert.Equal(DateTime.UtcNow.Year, created.Year);
    }

    [Fact]
    public async Task DuplicateRenavamIsRejected()
    {
        await CreateVehicleAsync(plate: "ABC1234", renavam: "11111111111");

        using var response = await PostVehicleAsync(NewVehicle(plate: "DEF5678", renavam: "11111111111"));

        // BUG CONHECIDO: o correto é 409 Conflict.
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DuplicatePlateIsRejectedOnlyByTheDatabase()
    {
        await CreateVehicleAsync(plate: "ABC1234");

        using var response = await PostVehicleAsync(NewVehicle(plate: "ABC1234"));

        // BUG CONHECIDO: o correto é 409. A API não confere a placa antes de gravar;
        // quem recusa é o índice único do banco, e a violação vira 500.
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Theory]
    [InlineData("AB12345")]
    [InlineData("ABCD123")]
    [InlineData("1234ABC")]
    public async Task InvalidPlateIsRejected(string plate)
    {
        using var response = await PostVehicleAsync(NewVehicle(plate: plate));

        // BUG CONHECIDO: o correto é 400 com o erro no campo "plate".
        // O domínio lança ArgumentException, que o tratamento global transforma em 500.
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Theory]
    [InlineData(1899)]
    [InlineData(0)]
    public async Task YearOutOfRangeIsRejected(int year)
    {
        using var response = await PostVehicleAsync(NewVehicle(year: year));

        // BUG CONHECIDO: o correto é 400 com o erro no campo "year".
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task FutureYearIsRejected()
    {
        using var response = await PostVehicleAsync(NewVehicle(year: DateTime.UtcNow.Year + 1));

        // BUG CONHECIDO: o correto é 400. Também vale rever a regra: modelo do ano seguinte é comum no Brasil.
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task MissingRequiredFieldsAreRejected()
    {
        using var response = await PostVehicleAsync(new { plate = "ABC1234", year = 2022 });

        // [ApiController] valida antes de chegar ao domínio, porque as propriedades string do request não são anuláveis.
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}
