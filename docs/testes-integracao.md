# Testes de integração

Os testes sobem a API inteira em memória (`WebApplicationFactory`) e falam com ela só por HTTP, contra um SQL Server de verdade num container descartável (Testcontainers). Nenhum teste chama handler ou repositório direto. Assim eles continuam valendo quando o código por dentro mudar, que é justamente o plano dos próximos PRs.

## Como funciona

1. **Um SQL Server por execução.** A imagem `mcr.microsoft.com/mssql/server:2022-CU27-ubuntu-22.04` sobe uma vez, antes do primeiro teste, e é removida no fim. A tag é fixa para que uma imagem nova não mude o resultado dos testes.
2. **Mesmo schema da produção.** Os testes leem o `database/changelogs/changelog-master.xml` e executam os scripts `sqlFile` na ordem do Liquibase. Um changeset de outro tipo faz os testes falharem com uma mensagem clara, em vez de montar um schema incompleto.
3. **Banco limpo antes de cada teste.** Os testes rodam em sequência, porque compartilham o banco, e cada um apaga as tabelas antes de começar. Os dados de que cada teste precisa são criados pela própria API (`POST /api/Vehicles`).

## Como rodar

Pré-requisitos: SDK do .NET 10 e Docker.

```bash
dotnet test --solution GeoTrack.sln
```

O `global.json` liga a Microsoft.Testing.Platform, que o `dotnet test` do .NET 10 exige para o xUnit v3.

### Docker dentro do WSL (sem Docker Desktop)

Se o Docker roda só no WSL, exponha o daemon numa porta local e aponte o Testcontainers para ela:

```powershell
$env:DOCKER_HOST = "tcp://127.0.0.1:2375"
dotnet test --solution GeoTrack.sln
```

Use `127.0.0.1`, não `localhost`. Com `localhost`, o SQL Client tenta primeiro o IPv6 (`::1`), esse caminho não chega ao SQL Server dentro do WSL, e a conexão expira com "O tempo limite de espera foi atingido".

### No GitHub Actions

O workflow `.github/workflows/ci.yml` roda restore, build e testes a cada push no `master` e em todo pull request. O runner `ubuntu-latest` já tem Docker, então não precisa de configuração extra.

## Bugs conhecidos registrados nos testes

Os testes registram o comportamento **atual** da API, inclusive o que está errado. Cada caso traz um comentário `BUG CONHECIDO` com o comportamento correto. Quando a correção entrar, o teste muda junto, e o diff do PR mostra exatamente o que mudou para quem usa a API.

| Cenário | Hoje | Correto |
|---|---|---|
| Veículo inexistente (GET, PUT) | 400 | 404 |
| Veículo inexistente ou já excluído (DELETE) | 200 `false` | 404 |
| Exclusão com sucesso | 200 `true` | 204 |
| Listagem sem resultados | 400 | 200 com lista vazia |
| RENAVAM duplicado | 400 | 409 |
| Placa duplicada | 500 (índice único do banco) | 409 |
| Placa inválida, ano fora do intervalo | 500 | 400 com o erro no campo |
| `offset` da listagem | pula registros, e as páginas se repetem | número da página, com `OrderBy` |
| Upload com placa desconhecida | `processedPositions` conta as posições ignoradas | contar só as gravadas |
| Upload com JSON inválido | devolve a mensagem interna do desserializador | só a mensagem de erro |

Para listar os casos no código: `grep -rn "BUG CONHECIDO" tests/`.
