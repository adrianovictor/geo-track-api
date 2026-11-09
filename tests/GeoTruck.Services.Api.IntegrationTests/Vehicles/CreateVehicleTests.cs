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
    public async Task DuplicateRenavamIsRejectedWithConflict()
    {
        await CreateVehicleAsync(plate: "ABC1234", renavam: "11111111111");

        using var response = await PostVehicleAsync(NewVehicle(plate: "DEF5678", renavam: "11111111111"));

        var problem = await AssertProblemAsync(response, HttpStatusCode.Conflict);
        Assert.Equal("Já existe um veículo com o RENAVAM 11111111111.", problem.Detail);
    }

    [Theory]
    [InlineData("ABC1234")]
    [InlineData("abc1234")] // a comparação é feita com a placa já em maiúsculas
    public async Task DuplicatePlateIsRejectedWithConflict(string plate)
    {
        await CreateVehicleAsync(plate: "ABC1234");

        using var response = await PostVehicleAsync(NewVehicle(plate: plate));

        var problem = await AssertProblemAsync(response, HttpStatusCode.Conflict);
        Assert.Equal("Já existe um veículo com a placa ABC1234.", problem.Detail);
    }

    [Fact]
    public async Task PlateOfADeletedVehicleIsStillInUse()
    {
        var deleted = await CreateVehicleAsync(plate: "ABC1234");
        using (await Client.DeleteAsync($"{VehiclesUrl}/{deleted.Id}", Ct)) { }

        using var response = await PostVehicleAsync(NewVehicle(plate: "ABC1234"));

        // A exclusão é lógica e o índice único do banco vale também para os excluídos.
        // Liberar a placa de um veículo excluído é decisão de produto, fora deste PR.
        await AssertProblemAsync(response, HttpStatusCode.Conflict);
    }

    [Theory]
    [InlineData("AB12345")]
    [InlineData("ABCD123")]
    [InlineData("1234ABC")]
    public async Task InvalidPlateIsRejectedOnThePlateField(string plate)
    {
        using var response = await PostVehicleAsync(NewVehicle(plate: plate));

        var messages = await AssertFieldErrorAsync(response, "plate");
        Assert.Contains("placa brasileira válida", Assert.Single(messages));
    }

    [Theory]
    [InlineData(1899)]
    [InlineData(0)]
    public async Task YearOutOfRangeIsRejectedOnTheYearField(int year)
    {
        using var response = await PostVehicleAsync(NewVehicle(year: year));

        await AssertFieldErrorAsync(response, "year");
    }

    [Fact]
    public async Task FutureYearIsRejectedOnTheYearField()
    {
        using var response = await PostVehicleAsync(NewVehicle(year: DateTime.UtcNow.Year + 1));

        // Regra atual do domínio. Vale rever: modelo do ano seguinte é comum no Brasil.
        var messages = await AssertFieldErrorAsync(response, "year");
        Assert.Equal($"O campo 'year' deve estar entre 1900 e {DateTime.UtcNow.Year}.", Assert.Single(messages));
    }

    [Fact]
    public async Task UnexpectedErrorsDoNotLeakDetails()
    {
        // Um RENAVAM maior que a coluna (VARCHAR 20) passa pelo domínio e só falha no banco:
        // é o caminho de um erro inesperado, que deve virar 500 sem expor a mensagem do SQL Server.
        // BUG CONHECIDO: o correto é 400 no campo "renavam". O tamanho dos campos (renavam 20,
        // model e brand 50) não é validado antes de gravar.
        using var response = await PostVehicleAsync(NewVehicle(renavam: new string('9', 200)));

        var problem = await AssertProblemAsync(response, HttpStatusCode.InternalServerError);
        Assert.DoesNotContain("truncated", problem.Detail ?? "", StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("String or binary", await response.Content.ReadAsStringAsync(Ct));
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
