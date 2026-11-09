using System.Net;
using GeoTruck.Services.Api.IntegrationTests.Infrastructure;

namespace GeoTruck.Services.Api.IntegrationTests.Cors;

/// <summary>
/// O front (geo-track-app) roda em http://localhost:3000. Sem o cabeçalho de CORS, o navegador
/// esconde a resposta e o axios não consegue ler o "detail" do erro, nem nas respostas de erro.
/// </summary>
public class CorsTests(GeoTrackApiFactory factory) : ApiTestBase(factory)
{
    private const string FrontOrigin = "http://localhost:3000";

    private async Task<HttpResponseMessage> SendFromFrontAsync(HttpMethod method, string url)
    {
        using var request = new HttpRequestMessage(method, url);
        request.Headers.Add("Origin", FrontOrigin);
        return await Client.SendAsync(request, Ct);
    }

    [Fact]
    public async Task SuccessfulResponseAllowsTheFront()
    {
        using var response = await SendFromFrontAsync(HttpMethod.Get, VehiclesUrl);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(FrontOrigin, Assert.Single(response.Headers.GetValues("Access-Control-Allow-Origin")));
    }

    [Fact]
    public async Task ErrorResponseFromTheDomainAllowsTheFront()
    {
        using var response = await SendFromFrontAsync(HttpMethod.Get, $"{VehiclesUrl}/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal(FrontOrigin, Assert.Single(response.Headers.GetValues("Access-Control-Allow-Origin")));
    }

    [Fact]
    public async Task OtherOriginsAreNotAllowed()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, VehiclesUrl);
        request.Headers.Add("Origin", "http://exemplo.com");
        using var response = await Client.SendAsync(request, Ct);

        Assert.False(response.Headers.Contains("Access-Control-Allow-Origin"));
    }
}
