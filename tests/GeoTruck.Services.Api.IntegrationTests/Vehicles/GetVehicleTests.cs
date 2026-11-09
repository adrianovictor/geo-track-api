using System.Net;
using GeoTruck.Services.Api.IntegrationTests.Infrastructure;

namespace GeoTruck.Services.Api.IntegrationTests.Vehicles;

public class GetVehicleTests(GeoTrackApiFactory factory) : ApiTestBase(factory)
{
    [Fact]
    public async Task ExistingVehicleReturns200()
    {
        var created = await CreateVehicleAsync(plate: "ABC1234");

        var vehicle = await GetVehicleAsync(created.Id);

        Assert.Equal(created.Id, vehicle.Id);
        Assert.Equal(created.UniqueId, vehicle.UniqueId);
        Assert.Equal("ABC1234", vehicle.Plate);
    }

    [Fact]
    public async Task MissingVehicleIsReported()
    {
        using var response = await Client.GetAsync($"{VehiclesUrl}/999999", Ct);

        // BUG CONHECIDO: o correto é 404 Not Found.
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeletedVehicleIsNotReturned()
    {
        var created = await CreateVehicleAsync();
        using (await Client.DeleteAsync($"{VehiclesUrl}/{created.Id}", Ct)) { }

        using var response = await Client.GetAsync($"{VehiclesUrl}/{created.Id}", Ct);

        // BUG CONHECIDO: o correto é 404 Not Found.
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
