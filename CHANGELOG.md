# Changelog

As mudanças de cada versão, da mais recente para a mais antiga. O formato segue o [Keep a Changelog](https://keepachangelog.com/pt-BR/1.1.0/), e as versões seguem o [Versionamento Semântico](https://semver.org/lang/pt-BR/).

## [Não lançado]

### Adicionado

- README, licença MIT, `CHANGELOG.md`, `CODEOWNERS`, templates de issue e Dependabot (NuGet e GitHub Actions).
- `.editorconfig` e verificação de formatação (`dotnet format`) no CI.
- O Dependabot não propõe versões major do MediatR: a partir da v13 ele tem licença comercial.

### Corrigido

- `.env.exemple` renomeado para `.env.example`, com a variável `DB_NAME` que o `docker-compose.yml` lê (antes era `DB_PERSONS_NAME`).

## [1.0.0] - 2026-11-09

Primeira versão marcada. Reúne a migração para o .NET 10 e a revisão do contrato de erros da API (PRs #1 a #5).

### Alterado

- Migração para o .NET 10 e o EF Core 10 (#1). Detalhes em [docs/migracao-dotnet-10.md](docs/migracao-dotnet-10.md).
- **Quebra de contrato:** todos os erros saem em `application/problem+json` (RFC 9457), no lugar de `{ success, message }` e `{ error, details }` (#3). Quem lia `error` no upload passa a ler `detail`.
- **Quebra de contrato:** a exclusão responde 204 sem corpo, no lugar de 200 com `true` (#3).
- O `offset` padrão da listagem passa de 1 para 0 (#4).
- As entidades usam a base e a auditoria da lib [core-common-components](https://github.com/adrianovictor/core-common-components) v0.1.0, que entra como submodule em `libs/` (#3, #4).
- Construtores primários e `record` posicional nos handlers e commands (#5).

### Adicionado

- Testes de integração com Testcontainers contra um SQL Server real, com o schema aplicado pelos scripts do Liquibase (#2). Detalhes em [docs/testes-integracao.md](docs/testes-integracao.md).
- CI no GitHub Actions: restore, build e testes a cada push no `master` e em todo pull request (#2).
- Template de descrição de pull request (#2).

### Corrigido

- Veículo inexistente ou excluído responde 404, no lugar de 400 (#3).
- Excluir um veículo inexistente ou já excluído responde 404, no lugar de 200 com `false` (#3).
- Listagem sem resultados responde 200 com a lista vazia, no lugar de 400 (#3).
- RENAVAM ou placa já usados por outro veículo respondem 409, no lugar de 400 ou 500 (#3).
- Placa inválida e ano fora do intervalo respondem 400 com o erro no campo, no lugar de 500 (#3).
- A paginação não repete mais veículos entre as páginas: `Skip(offset)` em ordem de `Id` (#4).
- `limit=0` na listagem não responde mais 500 por divisão por zero (#4).
- `processedPositions` no upload conta só as posições gravadas (#4).

### Segurança

- Fixa o `System.Security.Cryptography.Pkcs` em 10.0.14. A versão 9.0.11, vulnerável, vinha como dependência indireta do `Microsoft.Data.SqlClient` (#1).
- Erros inesperados e JSON inválido no upload não expõem mais a mensagem interna da exceção (#3).

[Não lançado]: https://github.com/adrianovictor/geo-track-api/compare/v1.0.0...HEAD
[1.0.0]: https://github.com/adrianovictor/geo-track-api/releases/tag/v1.0.0
