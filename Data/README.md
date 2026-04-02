# Data

Esta pasta e uma documentacao da camada `Data` da aplicacao.

Ela foi criada para descrever o que foi implementado em persistencia, acesso a dados e estrutura do banco, conforme solicitado no desafio.

## Objetivo da camada

A camada `Data` e responsavel por:

- acessar o SQL Server
- executar comandos SQL
- realizar leitura e escrita dos lancamentos
- mapear manualmente os dados do banco para objetos C#
- dar suporte a camada `Business`

## Requisitos de persistencia do desafio

- utilizar `SQL Server`
- criar o script SQL do banco
- criar as tabelas necessarias
- mapear manualmente os dados para objetos C#
- utilizar `ADO.NET puro`
- nao utilizar `ORM`

## O que foi implementado

### Acesso a dados

- repositorio em `ADO.NET puro`
- uso de `SqlConnection`
- uso de `SqlCommand`
- uso de `SqlDataReader`
- tratamento de duplicidade com excecao amigavel

### Consultas e operacoes

- insercao de lancamento
- atualizacao de lancamento
- atualizacao logica de status para pagamento e cancelamento
- consulta por `Id`
- listagem de lancamentos
- verificacao de duplicidade
- calculo de saldo com base em lancamentos `Pago`

### Banco de dados

- script para criacao do banco `DesafioPlenoDb`
- criacao da tabela `dbo.LancamentoFinanceiro`
- criacao de indice unico para evitar duplicidade por:
  - `Competencia`
  - `Descricao`
  - `Tipo`

## Arquivos relacionados

- [SqlLancamentoRepository.cs](C:\xampp\htdocs\desafioPleno\Data\Repositories\SqlLancamentoRepository.cs)
- [CreateDatabase.sql](C:\xampp\htdocs\desafioPleno\Data\Scripts\CreateDatabase.sql)

## Como a camada se comunica com o restante da aplicacao

- a camada `Business` depende da interface [ILancamentoRepository.cs](C:\xampp\htdocs\desafioPleno\WebApp\CodeBehind\Interfaces\ILancamentoRepository.cs)
- a classe [SqlLancamentoRepository.cs](C:\xampp\htdocs\desafioPleno\Data\Repositories\SqlLancamentoRepository.cs) implementa essa interface
- a aplicacao registra essa implementacao no `Program.cs` do front-end

## Observacao

Toda a persistencia do projeto foi implementada manualmente, sem Entity Framework e sem qualquer ORM, para manter aderencia ao enunciado do desafio.
