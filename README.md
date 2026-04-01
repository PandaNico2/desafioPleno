# desafioPleno

Aplicacao web para controle de lancamentos financeiros, criada para um desafio tecnico com `ASP.NET` no front-end, `C#` no back-end e `SQL Server` com `ADO.NET puro`.

## Decisao tecnica

Eu optei por desenvolver a aplicacao utilizando `ASP.NET Core Razor Pages` em vez de `Web Forms`.

O motivo e pratico: `Web Forms` pertence ao ecossistema mais antigo do `.NET Framework`, enquanto o ambiente atual do projeto esta em `.NET 10`. `Razor Pages` foi a alternativa mais proxima para manter uma interface simples, sem usar MVC completo, preservando:

- separacao em camadas
- back-end em C#
- persistencia com SQL Server
- acesso a dados com `SqlConnection`, `SqlCommand` e `SqlDataReader`
- ausencia de ORM

## Estrutura

- `WebApp/frontend/frontend`: aplicacao Razor Pages
- `WebApp/CodeBehind`: entidades, regras de negocio e servicos
- `Data`: persistencia ADO.NET e script SQL

## Funcionalidades

- cadastro de lancamento financeiro
- listagem com filtro por competencia
- totalizador de saldo considerando apenas lancamentos pagos
- edicao apenas de lancamentos em aberto
- pagamento logico com `DataPagamento`
- cancelamento logico com `DataCancelamento`
- exportacao por competencia em `CSV`

## Como executar

1. Execute o script `Data/Scripts/CreateDatabase.sql` no SQL Server.
2. Ajuste a connection string em `WebApp/frontend/frontend/appsettings.json` se necessario.
3. Rode os comandos:

```powershell
dotnet restore WebApp\frontend\frontend.slnx
dotnet run --project WebApp\frontend\frontend\frontend.csproj
```

## Regras implementadas

- credito usa desconto obrigatorio
- debito usa taxa obrigatoria
- taxa e desconto sao mutuamente exclusivos
- nao permite duplicidade por `Competencia + Descricao + Tipo`
- recalcula `ValorCalculado` ao editar lancamento em aberto
- apenas lancamentos `Pago` entram no saldo
- pagamento e cancelamento sao operacoes logicas

## Limitacoes

- a exportacao foi entregue em `CSV`, conforme permitido no enunciado
- o foco foi entrega funcional e validacao das regras de negocio
- o layout foi mantido simples para priorizar o prazo e a cobertura do fluxo principal
