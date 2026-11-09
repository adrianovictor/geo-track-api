using System.Net;
using System.Net.Http.Json;
using GeoTruck.Services.Api.IntegrationTests.Infrastructure;

namespace GeoTruck.Services.Api.IntegrationTests.Vehicles;

public class ListVehiclesTests(GeoTrackApiFactory factory) : ApiTestBase(factory)
{
    private async Task<VehicleListResponse> ListAsync(string query = "")
    {
        using var response = await Client.GetAsync($"{VehiclesUrl}{query}", Ct);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<VehicleListResponse>(Ct))!;
    }

    [Fact]
    public async Task ListsVehiclesWithTotals()
    {
        await CreateVehicleAsync(plate: "ABC1234");
        await CreateVehicleAsync(plate: "DEF5678");

        var list = await ListAsync();

        Assert.Equal(2, list.TotalRecords);
        Assert.Equal(["ABC1234", "DEF5678"], list.Vehicles.Select(v => v.Plate).Order());
        Assert.Equal(1, list.CurrentPage);
        Assert.Equal(10, list.PageItens);
    }

    [Fact]
    public async Task FiltersByPartOfThePlate()
    {
        await CreateVehicleAsync(plate: "CJL1234");
        await CreateVehicleAsync(plate: "CJL5678");
        await CreateVehicleAsync(plate: "XYZ1234");

        var list = await ListAsync("?plate=CJL");

        Assert.Equal(2, list.TotalRecords);
        Assert.All(list.Vehicles, v => Assert.StartsWith("CJL", v.Plate));
    }

    [Fact]
    public async Task FiltersByBrandModelAndYear()
    {
        await CreateVehicleAsync(plate: "ABC1234", brand: "Volvo", model: "FH 540", year: 2022);
        await CreateVehicleAsync(plate: "DEF5678", brand: "Volvo", model: "FH 460", year: 2019);
        await CreateVehicleAsync(plate: "GHI9012", brand: "Scania", model: "R 450", year: 2022);

        Assert.Equal(2, (await ListAsync("?brand=Volvo")).TotalRecords);
        Assert.Equal("DEF5678", Assert.Single((await ListAsync("?model=460")).Vehicles).Plate);
        Assert.Equal(2, (await ListAsync("?year=2022")).TotalRecords);
        Assert.Equal("ABC1234", Assert.Single((await ListAsync("?brand=Volvo&year=2022")).Vehicles).Plate);
    }

    [Fact]
    public async Task DeletedVehiclesAreNotListed()
    {
        await CreateVehicleAsync(plate: "ABC1234");
        var deleted = await CreateVehicleAsync(plate: "DEF5678");
        using (await Client.DeleteAsync($"{VehiclesUrl}/{deleted.Id}", Ct)) { }

        var list = await ListAsync();

        Assert.Equal("ABC1234", Assert.Single(list.Vehicles).Plate);
        Assert.Equal(1, list.TotalRecords);
    }

    [Fact]
    public async Task EmptyResultIsReported()
    {
        await CreateVehicleAsync(brand: "Volvo");

        using var response = await Client.GetAsync($"{VehiclesUrl}?brand=naoexiste", Ct);

        // BUG CONHECIDO: o correto é 200 com lista vazia. Lista vazia é resultado, não erro.
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task LimitControlsPageSize()
    {
        foreach (var plate in new[] { "AAA1111", "BBB2222", "CCC3333" })
            await CreateVehicleAsync(plate: plate);

        var list = await ListAsync("?limit=2");

        Assert.Equal(2, list.Vehicles.Count);
        Assert.Equal(3, list.TotalRecords);
        Assert.Equal(2, list.PageItens);
    }

    [Fact]
    public async Task OffsetSkipsRecordsInsteadOfPages()
    {
        foreach (var plate in new[] { "AAA1111", "BBB2222", "CCC3333", "DDD4444" })
            await CreateVehicleAsync(plate: plate);

        var first = await ListAsync("?limit=2&offset=1");
        var second = await ListAsync("?limit=2&offset=2");

        // BUG CONHECIDO: o "offset" é a posição do primeiro registro (começando em 1), não o número da página.
        // Com offset=2 a API pula só 1 registro, e as duas "páginas" repetem um veículo.
        // Além disso, a consulta não tem OrderBy, então a ordem dos registros não é garantida (aviso 10102 do EF).
        Assert.Equal(2, second.Vehicles.Count);
        Assert.Single(first.Vehicles.Select(v => v.Id).Intersect(second.Vehicles.Select(v => v.Id)));
    }
}
