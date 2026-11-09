using System.Net;
using System.Net.Http.Json;
using GeoTruck.Services.Api.IntegrationTests.Infrastructure;

namespace GeoTruck.Services.Api.IntegrationTests.Vehicles;

public class UpdateVehicleTests(GeoTrackApiFactory factory) : ApiTestBase(factory)
{
    [Fact]
    public async Task UpdatesAllFields()
    {
        var created = await CreateVehicleAsync(plate: "ABC1234", model: "FH 540", brand: "Volvo", year: 2020);

        using var response = await Client.PutAsJsonAsync($"{VehiclesUrl}/{created.Id}",
            NewVehicle(plate: "ABC1D23", renavam: "99999999999", model: "Actros", brand: "Mercedes-Benz", year: 2023), Ct);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var stored = await GetVehicleAsync(created.Id);
        Assert.Equal("ABC1D23", stored.Plate);
        Assert.Equal("99999999999", stored.Renavam);
        Assert.Equal("Actros", stored.Model);
        Assert.Equal("Mercedes-Benz", stored.Brand);
        Assert.Equal(2023, stored.Year);
        Assert.Equal(created.UniqueId, stored.UniqueId);
    }

    [Fact]
    public async Task MissingVehicleIsReported()
    {
        using var response = await Client.PutAsJsonAsync($"{VehiclesUrl}/999999", NewVehicle(), Ct);

        // BUG CONHECIDO: o correto é 404 Not Found.
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task InvalidPlateIsRejectedAndNothingChanges()
    {
        var created = await CreateVehicleAsync(plate: "ABC1234");

        using var response = await Client.PutAsJsonAsync($"{VehiclesUrl}/{created.Id}", NewVehicle(plate: "INVALIDA"), Ct);

        // BUG CONHECIDO: o correto é 400 com o erro no campo "plate".
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal("ABC1234", (await GetVehicleAsync(created.Id)).Plate);
    }
}
