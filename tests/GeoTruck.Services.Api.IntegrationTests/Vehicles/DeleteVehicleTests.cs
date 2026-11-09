using System.Net;
using System.Net.Http.Json;
using GeoTruck.Services.Api.IntegrationTests.Infrastructure;

namespace GeoTruck.Services.Api.IntegrationTests.Vehicles;

public class DeleteVehicleTests(GeoTrackApiFactory factory) : ApiTestBase(factory)
{
    [Fact]
    public async Task DeletesExistingVehicle()
    {
        var created = await CreateVehicleAsync();

        using var response = await Client.DeleteAsync($"{VehiclesUrl}/{created.Id}", Ct);

        // O correto seria 204 No Content. Hoje a API responde 200 com "true".
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(await response.Content.ReadFromJsonAsync<bool>(Ct));

        using var get = await Client.GetAsync($"{VehiclesUrl}/{created.Id}", Ct);
        Assert.NotEqual(HttpStatusCode.OK, get.StatusCode);
    }

    [Fact]
    public async Task MissingVehicleIsReportedAsSuccess()
    {
        using var response = await Client.DeleteAsync($"{VehiclesUrl}/999999", Ct);

        // BUG CONHECIDO: o correto é 404. O handler captura a própria VehicleNotFoundException
        // e devolve "false", que a API responde como 200.
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.False(await response.Content.ReadFromJsonAsync<bool>(Ct));
    }

    [Fact]
    public async Task DeletingTwiceIsReportedAsSuccess()
    {
        var created = await CreateVehicleAsync();
        using (await Client.DeleteAsync($"{VehiclesUrl}/{created.Id}", Ct)) { }

        using var response = await Client.DeleteAsync($"{VehiclesUrl}/{created.Id}", Ct);

        // BUG CONHECIDO: mesmo caso do veículo inexistente (o excluído não é encontrado).
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.False(await response.Content.ReadFromJsonAsync<bool>(Ct));
    }
}
