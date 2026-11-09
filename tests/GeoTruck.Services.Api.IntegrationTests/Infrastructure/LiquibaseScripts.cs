using System.Xml.Linq;
using Microsoft.Data.SqlClient;

namespace GeoTruck.Services.Api.IntegrationTests.Infrastructure;

/// <summary>
/// Monta o schema com os mesmos scripts SQL que o Liquibase aplica, na ordem do changelog.
/// Assim o banco dos testes é igual ao de produção, e não um schema gerado pelo EF.
/// </summary>
internal static class LiquibaseScripts
{
    private static readonly XNamespace Ns = "http://www.liquibase.org/xml/ns/dbchangelog";

    public static async Task ApplyAsync(string connectionString, CancellationToken ct)
    {
        var master = Path.Combine(AppContext.BaseDirectory, "changelogs", "changelog-master.xml");

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(ct);

        foreach (var script in ScriptsInOrder(master))
        {
            await using var command = new SqlCommand(await File.ReadAllTextAsync(script, ct), connection);
            await command.ExecuteNonQueryAsync(ct);
        }
    }

    private static IEnumerable<string> ScriptsInOrder(string changelogPath)
    {
        var directory = Path.GetDirectoryName(changelogPath)!;
        var root = XDocument.Load(changelogPath).Root!;

        foreach (var element in root.Elements())
        {
            if (element.Name == Ns + "include")
            {
                foreach (var script in ScriptsInOrder(Path.Combine(directory, (string)element.Attribute("file")!)))
                    yield return script;
            }
            else if (element.Name == Ns + "changeSet")
            {
                foreach (var change in element.Elements().Where(e => e.Name != Ns + "comment" && e.Name != Ns + "rollback"))
                {
                    if (change.Name == Ns + "sqlFile")
                        yield return Path.Combine(directory, (string)change.Attribute("path")!);
                    else if (change.Name != Ns + "tagDatabase")
                        // falha alto em vez de montar um schema incompleto sem ninguém perceber
                        throw new NotSupportedException(
                            $"Tipo de change do Liquibase sem suporte nos testes: <{change.Name.LocalName}>. Use sqlFile ou adicione o suporte aqui.");
                }
            }
        }
    }
}
