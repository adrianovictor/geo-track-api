## O que muda

<!-- Uma ou duas frases: o que este PR faz e por quê. Link da issue, se houver. -->

## Tipo

- [ ] Correção de bug
- [ ] Funcionalidade nova
- [ ] Refatoração (sem mudança de comportamento)
- [ ] Testes
- [ ] CI, build ou dependências
- [ ] Documentação

## Mudança de comportamento na API

<!-- Status HTTP, contrato de request/response ou regra de negócio que mudou para quem usa a API.
     Se corrigiu um caso marcado com BUG CONHECIDO, diga qual. Se nada mudou, escreva "Nenhuma". -->

| Endpoint / cenário | Antes | Depois |
|---|---|---|
|  |  |  |

## Como testar

<!-- Passos para conferir a mudança, além do CI. -->

```bash
dotnet test --solution GeoTrack.sln
```

## Checklist

- [ ] Build e testes passando (`dotnet test --solution GeoTrack.sln`)
- [ ] Testes novos ou atualizados para o comportamento alterado
- [ ] Comentários `BUG CONHECIDO` e a tabela em `docs/testes-integracao.md` atualizados, se algum bug foi corrigido
- [ ] Mudança de banco feita por changeset do Liquibase em `database/changelogs`, com rollback
- [ ] Nenhum segredo, connection string ou `.env` no diff
- [ ] Documentação atualizada, se necessário
