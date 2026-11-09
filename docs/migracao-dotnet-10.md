# Migração para .NET 10

**Data:** novembro de 2026 · **Branch:** `feature/migracao-dotnet-10`

A solução saiu do .NET 9 com EF Core 8 para o **.NET 10 com EF Core 10**. A migração troca só framework e pacotes: nenhuma regra de negócio mudou, e a API responde exatamente como antes, inclusive nos bugs já conhecidos (veja [O que fica para os próximos PRs](#o-que-fica-para-os-próximos-prs)).

## Por que migrar agora

| Versão | Tipo | Fim do suporte |
|---|---|---|
| .NET 8 / EF Core 8 | LTS | 10 de novembro de 2026 |
| .NET 9 | STS | 10 de novembro de 2026 |
| **.NET 10 / EF Core 10** | **LTS** | **novembro de 2028** |

O projeto usava as duas versões que perdem suporte na mesma semana: o .NET 9 no runtime e o EF Core 8 no acesso a dados. Depois dessa data, elas não recebem mais correções de segurança. O .NET 10 é a versão LTS atual, com três anos de suporte.

A migração veio **antes** das melhorias de código porque os testes de integração do próximo PR passam a ser escritos direto no .NET 10, sem configurar os pacotes de teste duas vezes.

## O que mudou

| Item | Antes | Depois |
|---|---|---|
| `TargetFramework` (5 projetos) | `net9.0` | `net10.0` |
| SDK fixado (`global.json`) | — | 10.0.100, aceitando bandas mais novas do 10 |
| `Microsoft.EntityFrameworkCore` (+ `.SqlServer`, `.Relational`, `.Design`) | 8.0.14 | 10.0.14 |
| `Microsoft.AspNetCore.OpenApi` | 9.0.9 | 10.0.14 |
| `Scalar.AspNetCore` | 2.8.10 | 2.19.0 |
| `Polly` | 8.6.4 | 8.9.1 |
| `MediatR` | 12.2.0 | 12.5.0 |
| `MediatR.Extensions.Microsoft.DependencyInjection` | 11.1.0 | removido |
| `Microsoft.AspNetCore.Http.Abstractions` | 2.3.0 | trocado por `FrameworkReference` |
| `Microsoft.Extensions.Configuration` | 9.0.8 | removido (vem com o framework) |
| `System.Security.Cryptography.Pkcs` | 9.0.11 (indireto) | 10.0.14 (fixado) |

Única mudança de código: `WithDarkMode()` do Scalar ficou obsoleto e virou `EnableDarkMode()` em `Program.cs`.

## Vulnerabilidades

### O que apareceu

No primeiro build após a atualização, a auditoria do NuGet (que roda a cada `restore`) acusou dois alertas:

| Alerta | Severidade | Falha |
|---|---|---|
| [GHSA-846f-f764-76mx](https://github.com/advisories/GHSA-846f-f764-76mx) (CVE-2026-84008) | Alta | Negação de serviço no .NET |
| [GHSA-2fmv-rv2c-59x7](https://github.com/advisories/GHSA-2fmv-rv2c-59x7) (CVE-2026-70162) | Moderada | Negação de serviço no .NET |

Os dois afetam o pacote `System.Security.Cryptography.Pkcs` até a 10.0.12 e foram corrigidos na **10.0.13**.

### De onde vinha

O projeto não referencia esse pacote. Ele chega por dependência indireta:

```
GeoTruck.Services.Infrastructure
└── Microsoft.EntityFrameworkCore.SqlServer 10.0.14
    └── Microsoft.Data.SqlClient 6.1.7
        └── System.Security.Cryptography.Pkcs 9.0.11   ← vulnerável
```

Comando usado para rastrear: `dotnet nuget why src/GeoTruck.Services.Api/GeoTruck.Services.Api.csproj System.Security.Cryptography.Pkcs`.

**A falha veio com a atualização.** O `master` (.NET 9 com EF Core 8) não tinha nenhum pacote vulnerável. Isso mostra por que a auditoria do NuGet deve ficar ligada: atualizar um pacote também atualiza a árvore inteira de dependências, inclusive o que ninguém escolheu diretamente.

### Como foi corrigido

O pacote foi fixado na versão corrigida, direto no projeto que traz a dependência (`GeoTruck.Services.Infrastructure.csproj`):

```xml
<PackageReference Include="System.Security.Cryptography.Pkcs" Version="10.0.14" />
```

Uma referência direta vence a indireta, então o NuGet passa a usar a 10.0.14 em toda a solução.

**Alternativa descartada:** subir o `Microsoft.Data.SqlClient` para a 7.x, que já traz a versão corrigida. É uma versão principal acima da que o EF Core 10 foi testado (6.1.x), um risco maior do que o problema que resolveria.

**Quando remover:** assim que uma versão do EF Core 10 passar a depender de um SqlClient com o pacote corrigido. Para conferir, retire a referência e rode o comando abaixo.

### Como verificar

```bash
dotnet list GeoTrack.sln package --vulnerable --include-transitive
```

Resultado após a correção: nenhum pacote vulnerável nos cinco projetos.

## Decisões

- **MediatR fica na 12.x.** A partir da 13, o MediatR passou a ter licença comercial. A 12.5.0 é a última com licença Apache 2.0. Se um dia for preciso sair dela, a alternativa é uma implementação própria, que é pequena para o que o projeto usa (envio de comandos e consultas para um handler).
- **Removido `MediatR.Extensions.Microsoft.DependencyInjection`.** Desde a 12, o próprio MediatR registra os handlers (`AddMediatR`). O pacote antigo exigia MediatR menor que 12 e gerava o aviso NU1608.
- **`FrameworkReference` no lugar de `Microsoft.AspNetCore.Http.Abstractions` 2.3.0.** O pacote era de 2018, da época do ASP.NET Core 2. A biblioteca CrossCutting usa tipos do ASP.NET Core (middleware), e o jeito atual de referenciá-los é o framework compartilhado.
- **Avisos de código ficaram de fora.** O build tem avisos de nulabilidade (CS8618, CS8625) e o `Vehicle.ChangeStatus` escondendo o método da base (CS0114). Eles já existiam e serão tratados nos PRs de melhoria. Assim, esta migração fica só migração e é fácil de revisar.

## Mudanças incompatíveis conferidas

Foram revisadas as mudanças incompatíveis (*breaking changes*) do EF Core 9 e 10 e do `Microsoft.Data.SqlClient` 6 contra o que o projeto usa:

| Recurso usado no projeto | Resultado |
|---|---|
| `Include` com `OrderBy` (posições do veículo) | Sem mudança |
| `OwnsOne` para a placa (`BrazilianPlate`) | Sem mudança |
| `Contains` em texto (filtros da listagem) | Sem mudança (continua `LIKE`) |
| `EnableRetryOnFailure` | Sem mudança |
| Criptografia da conexão (SqlClient 6 usa `Encrypt=True` por padrão) | Sem efeito: as connection strings já definem `Encrypt=False` e `TrustServerCertificate=True` |
| Migrações do EF | Não usadas: o schema é do Liquibase |

## Como foi validado

1. **Build:** 0 erros. Os avisos de pacote (NU1608, NU1902, NU1903) sumiram.
2. **Pacotes:** `dotnet list package --vulnerable --include-transitive` sem nenhum vulnerável.
3. **API no ar** (.NET 10, ambiente Development, SQL Server 2019 local), só com leituras e um POST inválido que não grava nada, para não alterar os dados:

| Chamada | Resposta | Observação |
|---|---|---|
| `GET /api/Vehicles` | 200 | Lista paginada |
| `GET /api/Vehicles?plate=CJL&limit=2` | 200 | Filtro por placa |
| `GET /api/Vehicles/7` | 200 | 18 posições, igual ao banco (testa o `Include`) |
| `GET /openapi/v1.json` | 200 | OpenAPI 3.1 |
| `GET /scalar/v1` | 200 | Documentação |
| `GET /api/Vehicles?brand=naoexiste` | 400 | Comportamento anterior (bug conhecido) |
| `GET /api/Vehicles/9999` | 400 | Comportamento anterior (bug conhecido) |
| `POST /api/Vehicles` com placa inválida | 500 | Comportamento anterior (bug conhecido) |

Cadastro, alteração, exclusão e upload de posições não foram testados manualmente, para não alterar a base local. Eles entram nos testes de integração do próximo PR, que rodam num banco descartável.

## O que fica para os próximos PRs

Encontrado durante a migração, sem relação com ela:

- **Paginação sem `OrderBy`.** O próprio EF avisa no log (`Microsoft.EntityFrameworkCore.Query[10102]`): `Skip`/`Take` sem ordenação podem repetir ou pular registros entre páginas.
- **Status HTTP incorretos.** Lista vazia e veículo inexistente respondem 400; entrada inválida responde 500.
- **Configuração local.**
  - O `appsettings.Development.json` aponta para o banco `GeoTrackeDb` (com "e" a mais), mas o banco se chama `GeoTrackDb`.
  - A connection string usa `127.0.0.1`, que só funciona com o TCP/IP do SQL Server habilitado.
  - A senha do `sa` está versionada no arquivo. O ideal é tirar credenciais do `appsettings` e usar [user-secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) ou variável de ambiente.

## Como rodar localmente

Pré-requisito: SDK do .NET 10. O `global.json` aceita qualquer versão 10.0.1xx ou mais nova.

Sem alterar arquivo nenhum, a connection string pode vir de variável de ambiente. Exemplo com login do Windows e o SQL Server local:

```bash
ConnectionStrings__DefaultConnection="Server=.;Database=GeoTrackDb;Integrated Security=True;TrustServerCertificate=True;Encrypt=False" \
  dotnet run --project src/GeoTruck.Services.Api --launch-profile http
```

A documentação da API fica em `http://localhost:5108/scalar/v1`.

Quem não tem SQL Server instalado pode subir o banco pelo `docker-compose.yml` da raiz. Copie o `.env.example` para `.env` e preencha os valores (o nome do banco vai em `DB_NAME`).
