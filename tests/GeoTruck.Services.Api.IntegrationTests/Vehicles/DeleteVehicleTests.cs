using System.Net;
using GeoTruck.Services.Api.IntegrationTests.Infrastructure;

namespace GeoTruck.Services.Api.IntegrationTests.Vehicles;

public class DeleteVehicleTests(GeoTrackApiFactory factory) : ApiTestBase(factory)
{
    [Fact]
    public async Task DeletesExistingVehicleWith204()
    {
        var created = await CreateVehicleAsync();

        using var response = await Client.DeleteAsync($"{VehiclesUrl}/{created.Id}", Ct);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Empty(await response.Content.ReadAsByteArrayAsync(Ct));

        using var get = await Client.GetAsync($"{VehiclesUrl}/{created.Id}", Ct);
        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
    }

    [Fact]
    public async Task MissingVehicleReturns404()
    {
        using var response = await Client.DeleteAsync($"{VehiclesUrl}/999999", Ct);

        var problem = await AssertProblemAsync(response, HttpStatusCode.NotFound);
        Assert.Equal("Veículo com ID 999999 não encontrado.", problem.Detail);
    }

    [Fact]
    public async Task DeletingTwiceReturns404()
    {
        var created = await CreateVehicleAsync();
        using (await Client.DeleteAsync($"{VehiclesUrl}/{created.Id}", Ct)) { }

        using var response = await Client.DeleteAsync($"{VehiclesUrl}/{created.Id}", Ct);

        // O veículo excluído não é encontrado, então a segunda exclusão é um 404.
        await AssertProblemAsync(response, HttpStatusCode.NotFound);
    }
}
