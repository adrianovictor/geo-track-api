using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace GeoTruck.Services.Api.IntegrationTests.Infrastructure;

/// <summary>
/// Base dos testes: banco limpo antes de cada teste e atalhos para chamar a API.
/// Os testes só falam HTTP com a API. Nada de chamar handler ou repositório direto:
/// assim eles continuam valendo quando o código por dentro mudar.
/// </summary>
[Collection(ApiCollection.Name)]
public abstract class ApiTestBase(GeoTrackApiFactory factory) : IAsyncLifetime
{
    protected const string VehiclesUrl = "/api/Vehicles";
    protected const string UploadUrl = "/api/Vehicles/upload-vehicles-route-positions";

    private static int _renavamSequence;

    protected HttpClient Client { get; } = factory.CreateClient();
    protected static CancellationToken Ct => TestContext.Current.CancellationToken;

    public async ValueTask InitializeAsync() => await factory.ResetDatabaseAsync();

    public ValueTask DisposeAsync()
    {
        Client.Dispose();
        return ValueTask.CompletedTask;
    }

    protected static string NewRenavam() =>
        (10_000_000_000L + Interlocked.Increment(ref _renavamSequence)).ToString();

    protected static object NewVehicle(
        string plate = "ABC1234",
        string? renavam = null,
        string model = "FH 540",
        string brand = "Volvo",
        int year = 2022) =>
        new { plate, renavam = renavam ?? NewRenavam(), model, brand, year };

    protected Task<HttpResponseMessage> PostVehicleAsync(object vehicle) =>
        Client.PostAsJsonAsync(VehiclesUrl, vehicle, Ct);

    /// <summary>Cadastra pela API e devolve o veículo criado. Falha o teste se o cadastro não der 201.</summary>
    protected async Task<VehicleResponse> CreateVehicleAsync(
        string plate = "ABC1234", string? renavam = null, string model = "FH 540", string brand = "Volvo", int year = 2022)
    {
        using var response = await PostVehicleAsync(NewVehicle(plate, renavam, model, brand, year));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<VehicleResponse>(Ct))!;
    }

    protected async Task<VehicleResponse> GetVehicleAsync(int id)
    {
        using var response = await Client.GetAsync($"{VehiclesUrl}/{id}", Ct);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<VehicleResponse>(Ct))!;
    }

    protected Task<HttpResponseMessage> UploadAsync(string json, string fileName = "posicoes.json", string contentType = "application/json")
    {
        var file = new ByteArrayContent(Encoding.UTF8.GetBytes(json));
        file.Headers.ContentType = new MediaTypeHeaderValue(contentType);

        var form = new MultipartFormDataContent { { file, "file", fileName } };
        return Client.PostAsync(UploadUrl, form, Ct);
    }

    /// <summary>
    /// Confere que a resposta é um erro em ProblemDetails (RFC 9457) com o status esperado e devolve o corpo.
    /// </summary>
    protected static async Task<ProblemResponse> AssertProblemAsync(HttpResponseMessage response, HttpStatusCode status)
    {
        Assert.Equal(status, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        var problem = await response.Content.ReadFromJsonAsync<ProblemResponse>(Ct);
        Assert.NotNull(problem);
        Assert.Equal((int)status, problem.Status);
        return problem;
    }

    /// <summary>Confere um 400 de validação com o erro no campo informado e devolve as mensagens do campo.</summary>
    protected static async Task<string[]> AssertFieldErrorAsync(HttpResponseMessage response, string field)
    {
        var problem = await AssertProblemAsync(response, HttpStatusCode.BadRequest);
        Assert.NotNull(problem.Errors);
        Assert.True(problem.Errors.TryGetValue(field, out var messages), $"Esperava erro no campo '{field}'.");
        return messages;
    }
}

public sealed record ProblemResponse(
    int Status, string? Title, string? Detail, Dictionary<string, string[]>? Errors);

public sealed record VehicleResponse(
    int Id, Guid UniqueId, string Plate, string Model, string Brand, int Year, string Renavam, List<LocationResponse> Locations);

public sealed record LocationResponse(
    int Id, int VehicleId, decimal Latitude, decimal Longitude, long PositionId, DateTime Date, DateTime DateUTC);

public sealed record VehicleListResponse(
    List<VehicleResponse> Vehicles, int TotalRecords, int CurrentPage, int PageItens);
