# Business

Esta pasta e uma documentacao da camada `Business` da aplicacao.

Ela foi criada para descrever o que foi implementado em regras de negocio, validacoes e comunicacao entre camadas, conforme solicitado no desafio.

## Objetivo da camada

A camada `Business` e responsavel por:

- aplicar as regras de negocio do sistema
- validar os dados recebidos da interface
- calcular o `ValorCalculado`
- controlar o fluxo de cadastro, edicao, pagamento e cancelamento
- se comunicar com a camada `Data` por meio de um repositorio

## Onde a implementacao foi feita

Embora esta pasta seja apenas documental, a implementacao da camada de negocio foi realizada na class library [CodeBehind](C:\xampp\htdocs\desafioPleno\WebApp\CodeBehind\CodeBehind.csproj).

O principal arquivo desta implementacao e [LancamentoService.cs](C:\xampp\htdocs\desafioPleno\WebApp\CodeBehind\Services\LancamentoService.cs).

## Requisitos de negocio do desafio

### Tipo de lancamento

- `Credito` soma no saldo
- `Debito` subtrai do saldo

### Regra de calculo

- percentual de taxa representa acrescimo sobre o valor original
- percentual de desconto representa reducao sobre o valor original
- taxa e desconto sao mutuamente exclusivos
- taxa somente para `Debito`
- desconto somente para `Credito`

### Persistencia e validacao

- `DataPagamento` e `DataCancelamento` devem ser informadas apenas em seus respectivos processos
- as demais informacoes sao obrigatorias na inclusao e na edicao
- nao pode existir lancamento duplicado com a mesma `Competencia`, `Descricao` e `Tipo`
- sempre que um lancamento em status `Aberto` for editado, o `ValorCalculado` deve ser recalculado

### Status do lancamento

- apenas lancamentos com status `Pago` devem ser considerados no calculo do saldo

## O que foi implementado

### Regras de negocio

- calculo de `ValorCalculado` para `Credito` e `Debito`
- regra de saldo considerando apenas lancamentos `Pago`
- bloqueio de edicao para lancamentos com status diferente de `Aberto`
- fluxo separado para pagamento e cancelamento

### Validacoes

- descricao obrigatoria
- competencia obrigatoria e no formato `AAAA-MM`
- `ValorOriginal` maior que zero
- taxa entre `0` e `100`
- desconto entre `0` e `100`
- `Debito` exige taxa
- `Credito` exige desconto
- `Credito` nao aceita taxa
- `Debito` nao aceita desconto
- validacao de duplicidade por `Competencia + Descricao + Tipo`

### Comunicacao entre camadas

- a interface envia os dados para a camada `Business`
- a camada `Business` valida e aplica as regras
- a camada `Business` chama a camada `Data` por meio da interface [ILancamentoRepository.cs](C:\xampp\htdocs\desafioPleno\WebApp\CodeBehind\Interfaces\ILancamentoRepository.cs)
- a camada `Data` executa a persistencia no SQL Server

## Arquivos relacionados

- [LancamentoFinanceiro.cs](C:\xampp\htdocs\desafioPleno\WebApp\CodeBehind\Entities\LancamentoFinanceiro.cs)
- [LancamentoInputModel.cs](C:\xampp\htdocs\desafioPleno\WebApp\CodeBehind\Models\LancamentoInputModel.cs)
- [DomainException.cs](C:\xampp\htdocs\desafioPleno\WebApp\CodeBehind\Models\DomainException.cs)
- [ILancamentoRepository.cs](C:\xampp\htdocs\desafioPleno\WebApp\CodeBehind\Interfaces\ILancamentoRepository.cs)
- [LancamentoService.cs](C:\xampp\htdocs\desafioPleno\WebApp\CodeBehind\Services\LancamentoService.cs)

## Observacao

O nome fisico do projeto esta como `CodeBehind`, mas sua funcao arquitetural dentro da solucao corresponde a camada `Business`.
