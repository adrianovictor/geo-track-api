using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Testcontainers.MsSql;

namespace GeoTruck.Services.Api.IntegrationTests.Infrastructure;

/// <summary>
/// Sobe a API inteira em memória, apontando para um SQL Server descartável num container.
/// Um container para todos os testes; cada teste limpa as tabelas antes de rodar.
/// </summary>
public sealed class GeoTrackApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    // Mesma versão principal do docker-compose. A tag fixa evita que uma imagem nova mude o resultado dos testes.
    private const string SqlServerImage = "mcr.microsoft.com/mssql/server:2022-CU27-ubuntu-22.04";
    private const string DatabaseName = "GeoTrackTests";

    private readonly MsSqlContainer _sqlServer = new MsSqlBuilder(SqlServerImage).Build();
    private string _connectionString = string.Empty;

    public async ValueTask InitializeAsync()
    {
        var ct = TestContext.Current.CancellationToken;
        await _sqlServer.StartAsync(ct);

        await using (var master = new SqlConnection(_sqlServer.GetConnectionString()))
        {
            await master.OpenAsync(ct);
            await using var create = new SqlCommand($"CREATE DATABASE [{DatabaseName}]", master);
            await create.ExecuteNonQueryAsync(ct);
        }

        _connectionString = new SqlConnectionStringBuilder(_sqlServer.GetConnectionString())
        {
            InitialCatalog = DatabaseName,
        }.ConnectionString;

        await LiquibaseScripts.ApplyAsync(_connectionString, ct);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("ConnectionStrings:DefaultConnection", _connectionString);
    }

    public async Task ResetDatabaseAsync()
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand("DELETE FROM VehicleLocations; DELETE FROM Vehicles;", connection);
        await command.ExecuteNonQueryAsync();
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        await _sqlServer.DisposeAsync();
    }
}

[CollectionDefinition(Name)]
public sealed class ApiCollection : ICollectionFixture<GeoTrackApiFactory>
{
    // Todos os testes compartilham o banco, então rodam em sequência dentro desta coleção.
    public const string Name = "API";
}
