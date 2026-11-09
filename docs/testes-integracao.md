# Testes de integração

Os testes sobem a API inteira em memória (`WebApplicationFactory`) e falam com ela só por HTTP, contra um SQL Server de verdade num container descartável (Testcontainers). Nenhum teste chama handler ou repositório direto. Assim eles continuam valendo quando o código por dentro mudar, que é justamente o plano dos próximos PRs.

## Como funciona

1. **Um SQL Server por execução.** A imagem `mcr.microsoft.com/mssql/server:2022-CU27-ubuntu-22.04` sobe uma vez, antes do primeiro teste, e é removida no fim. A tag é fixa para que uma imagem nova não mude o resultado dos testes.
2. **Mesmo schema da produção.** Os testes leem o `database/changelogs/changelog-master.xml` e executam os scripts `sqlFile` na ordem do Liquibase. Um changeset de outro tipo faz os testes falharem com uma mensagem clara, em vez de montar um schema incompleto.
3. **Banco limpo antes de cada teste.** Os testes rodam em sequência, porque compartilham o banco, e cada um apaga as tabelas antes de começar. Os dados de que cada teste precisa são criados pela própria API (`POST /api/Vehicles`).

## Como rodar

Pré-requisitos: SDK do .NET 10 e Docker.

A lib `core-common-components` entra como submodule em `libs/`. Num clone novo, baixe-a antes do build:

```bash
git submodule update --init   # ou clone com: git clone --recurse-submodules
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

## Formato dos erros

Todo erro sai em `application/problem+json` ([RFC 9457](https://www.rfc-editor.org/rfc/rfc9457)), pelo tratamento da lib `core-common-components` (`AddDomainProblemDetails`). A mensagem para o usuário fica em `detail`. Nos erros de validação, `errors` traz as mensagens por campo:

```json
{
  "status": 400,
  "title": "Dados inválidos",
  "detail": "O campo 'plate' não é uma placa brasileira válida (ABC1234, ABC-1234 ou ABC1D23).",
  "errors": { "plate": ["O campo 'plate' não é uma placa brasileira válida (ABC1234, ABC-1234 ou ABC1D23)."] }
}
```

| Situação | Status |
|---|---|
| Campo inválido (domínio ou `[ApiController]`) | 400 com `errors` |
| Veículo inexistente ou excluído | 404 |
| RENAVAM ou placa já usados por outro veículo, inclusive excluído | 409 |
| Outra regra de negócio | 422 |
| Erro inesperado | 500, sem a mensagem interna |

## Bugs conhecidos registrados nos testes

Os testes registram o comportamento **atual** da API, inclusive o que está errado. Cada caso traz um comentário `BUG CONHECIDO` com o comportamento correto. Quando a correção entrar, o teste muda junto, e o diff do PR mostra exatamente o que mudou para quem usa a API.

| Cenário | Hoje | Correto |
|---|---|---|
| Campo maior que a coluna (RENAVAM 20, modelo e marca 50) | 500 | 400 com o erro no campo |

Corrigidos no PR 3: os status de veículo inexistente (404), exclusão (204), listagem vazia (200), duplicidade (409), validação (400 com `errors`) e a mensagem interna do desserializador no upload.

Corrigidos no PR 4:

- **Paginação:** o `offset` passou a ser a quantidade de registros a pular (`Skip(offset)`, padrão 0), com ordem por `Id`. As páginas que o front pede (`Offset = (página - 1) * Limit`) não se repetem mais.
- **Valores fora do intervalo:** `limit` ≤ 0 e `offset` negativo passaram a ser corrigidos. Antes, `limit=0` respondia 500, por divisão por zero.
- **Upload:** `processedPositions` conta só as posições gravadas.

Para listar os casos no código: `grep -rn "BUG CONHECIDO" tests/`.
