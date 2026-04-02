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
- listagem com filtro por competencia, tipo e status
- totalizador de saldo considerando apenas lancamentos pagos
- edicao apenas de lancamentos em aberto
- pagamento logico com `DataPagamento`
- cancelamento logico com `DataCancelamento`
- exportacao em `CSV`

## Pre-requisitos

- `.NET SDK 10`
- `SQL Server` ou `LocalDB`
- `Visual Studio 2022` ou terminal com `dotnet`

## Como executar

### 1. Criar o banco de dados

Execute o script [CreateDatabase.sql](C:\xampp\htdocs\desafioPleno\Data\Scripts\CreateDatabase.sql) no seu SQL Server.

Se preferir, voce pode criar o banco manualmente e depois rodar apenas a criacao da tabela e do indice unico.

### 2. Conferir a connection string

Abra [appsettings.json](C:\xampp\htdocs\desafioPleno\WebApp\frontend\frontend\appsettings.json) e ajuste a conexao se necessario.

Exemplo atual:

```json
"DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=DesafioPlenoDb;Trusted_Connection=True;TrustServerCertificate=True;"
```

Se estiver usando `SQLEXPRESS`, um exemplo seria:

```json
"DefaultConnection": "Server=.\\SQLEXPRESS;Database=DesafioPlenoDb;Trusted_Connection=True;TrustServerCertificate=True;"
```

### 3. Restaurar os pacotes

No terminal, na raiz do projeto, execute:

```powershell
dotnet restore WebApp\frontend\frontend\frontend.csproj
```

### 4. Executar a aplicacao

Ainda na raiz do projeto, execute:

```powershell
dotnet run --project WebApp\frontend\frontend\frontend.csproj
```

### 5. Abrir no navegador

Depois de iniciar a aplicacao, acesse:

- [http://localhost:5000](http://localhost:5000)

## Como validar rapidamente

1. Abra a tela inicial.
2. Clique em `Novo lancamento`.
3. Cadastre um `Credito` com desconto ou um `Debito` com taxa.
4. Verifique se o lancamento aparece na listagem.
5. Teste as acoes de editar, pagar/receber, cancelar e exportar CSV.

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
