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
    public async Task KeepingItsOwnPlateAndRenavamIsAllowed()
    {
        var created = await CreateVehicleAsync(plate: "ABC1234", renavam: "11111111111");

        using var response = await Client.PutAsJsonAsync($"{VehiclesUrl}/{created.Id}",
            NewVehicle(plate: "ABC1234", renavam: "11111111111", model: "FH 460"), Ct);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("FH 460", (await GetVehicleAsync(created.Id)).Model);
    }

    [Fact]
    public async Task MissingVehicleReturns404()
    {
        using var response = await Client.PutAsJsonAsync($"{VehiclesUrl}/999999", NewVehicle(), Ct);

        await AssertProblemAsync(response, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PlateOfAnotherVehicleIsRejectedWithConflict()
    {
        await CreateVehicleAsync(plate: "ABC1234");
        var other = await CreateVehicleAsync(plate: "DEF5678");

        using var response = await Client.PutAsJsonAsync($"{VehiclesUrl}/{other.Id}", NewVehicle(plate: "ABC1234"), Ct);

        await AssertProblemAsync(response, HttpStatusCode.Conflict);
        Assert.Equal("DEF5678", (await GetVehicleAsync(other.Id)).Plate);
    }

    [Fact]
    public async Task RenavamOfAnotherVehicleIsRejectedWithConflict()
    {
        await CreateVehicleAsync(plate: "ABC1234", renavam: "11111111111");
        var other = await CreateVehicleAsync(plate: "DEF5678", renavam: "22222222222");

        using var response = await Client.PutAsJsonAsync($"{VehiclesUrl}/{other.Id}",
            NewVehicle(plate: "DEF5678", renavam: "11111111111"), Ct);

        await AssertProblemAsync(response, HttpStatusCode.Conflict);
        Assert.Equal("22222222222", (await GetVehicleAsync(other.Id)).Renavam);
    }

    [Fact]
    public async Task InvalidPlateIsRejectedOnThePlateFieldAndNothingChanges()
    {
        var created = await CreateVehicleAsync(plate: "ABC1234");

        using var response = await Client.PutAsJsonAsync($"{VehiclesUrl}/{created.Id}", NewVehicle(plate: "INVALIDA"), Ct);

        await AssertFieldErrorAsync(response, "plate");
        Assert.Equal("ABC1234", (await GetVehicleAsync(created.Id)).Plate);
    }
}
