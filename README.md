# GeoTrack API

[![CI](https://github.com/adrianovictor/geo-track-api/actions/workflows/ci.yml/badge.svg)](https://github.com/adrianovictor/geo-track-api/actions/workflows/ci.yml)
[![.NET 10](https://img.shields.io/badge/.NET-10-512BD4)](https://dotnet.microsoft.com/)
[![Licença MIT](https://img.shields.io/badge/licen%C3%A7a-MIT-blue)](LICENSE)

API REST para cadastro de veículos e rastreamento das rotas que eles percorrem. Os veículos são cadastrados com placa (padrão antigo ou Mercosul) e RENAVAM. As posições de GPS chegam num arquivo JSON e ficam ligadas a cada veículo, para o front desenhar a rota no mapa.

O front que consome a API está em [geo-track-app](https://github.com/adrianovictor/geo-track-app).

## Stack

| Camada | Tecnologia |
|---|---|
| API | ASP.NET Core 10, OpenAPI + [Scalar](https://scalar.com/) |
| Aplicação | MediatR 12 (commands e queries) |
| Dados | EF Core 10, SQL Server 2022, Polly (retentativa nas leituras) |
| Banco | Liquibase (scripts SQL versionados em `database/changelogs`) |
| Testes | xUnit v3, `WebApplicationFactory`, Testcontainers |
| CI | GitHub Actions |
| Lib comum | [core-common-components](https://github.com/adrianovictor/core-common-components) (submodule em `libs/`) |

## Estrutura

```
src/
├── GeoTruck.Services.Api             # controllers, configuração e tratamento de erros
├── GeoTruck.Services.Application     # commands, queries e DTOs (MediatR)
├── GeoTruck.Services.Domain          # entidades, value objects, exceções e contratos de repositório
├── GeoTruck.Services.Infrastructure  # EF Core, mapeamentos e repositórios
└── GeoTruck.Services.CrossCutting    # registro de dependências
tests/
└── GeoTruck.Services.Api.IntegrationTests
libs/
└── core-common-components            # submodule: entidade base, exceções e ProblemDetails
database/
└── changelogs                        # schema do banco (Liquibase)
```

## Como rodar

Pré-requisitos: SDK do .NET 10 e Docker.

**1. Clonar com o submodule**

```bash
git clone --recurse-submodules https://github.com/adrianovictor/geo-track-api.git
cd geo-track-api
```

Se já clonou sem o `--recurse-submodules`, rode `git submodule update --init`.

**2. Subir o banco**

```bash
cp .env.example .env   # preencha as senhas e o nome do banco (DB_NAME)
docker compose up -d
```

O compose sobe o SQL Server e cria o banco, ou restaura o backup `<DB_NAME>.bak` da pasta `BACKUPS_DIR`, se ele existir. Depois aplica o schema com o Liquibase.

**3. Rodar a API**

```bash
# A connection string vem do ambiente, para a senha não ficar em arquivo
export ConnectionStrings__DefaultConnection="Server=127.0.0.1;Database=<DB_NAME>;User Id=<ADMIN_USER>;Password=<ADMIN_PASSWORD>;TrustServerCertificate=True"
dotnet run --project src/GeoTruck.Services.Api
```

No PowerShell, use `$env:ConnectionStrings__DefaultConnection = "..."`.

A API sobe em `http://localhost:5108`. Em desenvolvimento, a documentação interativa fica em `http://localhost:5108/scalar`, e a especificação OpenAPI em `/openapi/v1.json`.

## Endpoints

| Método | Rota | O que faz |
|---|---|---|
| `GET` | `/api/Vehicles` | Lista veículos. Filtros: `renavam`, `plate`, `model`, `brand`, `year`. Paginação: `limit` (padrão 10) e `offset` (registros a pular, padrão 0) |
| `GET` | `/api/Vehicles/{id}` | Detalhes do veículo, com as posições da rota |
| `POST` | `/api/Vehicles` | Cadastra um veículo |
| `PUT` | `/api/Vehicles/{id}` | Altera um veículo |
| `DELETE` | `/api/Vehicles/{id}` | Exclui um veículo (exclusão lógica). Responde 204 |
| `POST` | `/api/Vehicles/upload-vehicles-route-positions` | Recebe um arquivo JSON com posições de GPS (até 10 MB e 10.000 posições) |

Todo erro sai em `application/problem+json` ([RFC 9457](https://www.rfc-editor.org/rfc/rfc9457)), com a mensagem em `detail` e, nos erros de validação, as mensagens por campo em `errors`. A tabela de status está em [docs/testes-integracao.md](docs/testes-integracao.md#formato-dos-erros).

## Testes

```bash
dotnet test --solution GeoTrack.sln
```

Os testes sobem a API em memória e falam com ela só por HTTP, contra um SQL Server de verdade num container descartável. O schema vem dos mesmos scripts do Liquibase usados no banco. Precisa do Docker rodando. Detalhes, inclusive como rodar com o Docker dentro do WSL, em [docs/testes-integracao.md](docs/testes-integracao.md).

## Como contribuir

- **Branch:** crie a partir do `master` (`feature/...`, `fix/...`, `refactor/...`, `chore/...`).
- **Commits:** sigam o [Conventional Commits](https://www.conventionalcommits.org/pt-br/v1.0.0/): `feat:`, `fix:`, `refactor:`, `test:`, `docs:`, `ci:`, `build:`, `chore:`.
- **Pull request:** o template já traz as seções. Mudança de comportamento na API entra na tabela "Antes / Depois".
- **CI:** o PR só entra com build, formatação e testes passando. Para formatar localmente: `dotnet format GeoTrack.sln --exclude libs/`.
- **Mudanças:** registre em `CHANGELOG.md`, na seção "Não lançado".

## Documentação

- [Migração para o .NET 10](docs/migracao-dotnet-10.md): o que mudou, a vulnerabilidade corrigida e como foi testado.
- [Testes de integração](docs/testes-integracao.md): como funcionam, como rodar, o formato dos erros e os bugs conhecidos.
- [CHANGELOG](CHANGELOG.md)

## Licença

[MIT](LICENSE)
