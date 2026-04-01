# Checklist do Projeto

Este arquivo foi revisado contra os requisitos informados do desafio tecnico:

- aplicacao web simples com `ASP.NET`
- back-end em `C#`
- persistencia em `SQL Server`
- acesso com `ADO.NET puro`
- sem `MVC`
- sem `ORM`
- entidade principal `LancamentoFinanceiro`
- cadastro, listagem, edicao, pagamento, cancelamento e exportacao

## Legenda de Status

- `[x]` Feito
- `[-]` Em andamento
- `[ ]` Falta fazer

## 1. Estrutura e Decisao Tecnica

### Requisitos do desafio
- usar `ASP.NET` no front-end
- usar `C#` em uma class library no back-end
- usar `SQL Server`
- usar `ADO.NET puro`
- nao usar `MVC`
- nao usar `ORM`
- seguir padrao arquitetural simples

### Status
- [x] Back-end separado em class library
- [x] Persistencia separada em camada propria
- [x] Uso de `SqlConnection`, `SqlCommand` e `SqlDataReader`
- [x] Nao usar ORM
- [x] Estrutura em camadas simples
- [x] Front-end entregue com `Razor Pages` em vez de `Web Forms`

### Observacao para entrega
- [x] README explica a decisao de usar `Razor Pages` como adaptacao pratica ao ambiente atual

## 2. Banco de Dados

### Requisitos do desafio
- criar script SQL do banco
- criar as tabelas necessarias
- impedir lancamento duplicado por `Competencia + Descricao + Tipo`

### Status de desenvolvimento
- [x] Script SQL criado
- [x] Banco `DesafioPlenoDb` criado e ajustado no ambiente local
- [x] Tabela `dbo.LancamentoFinanceiro` criada e validada no ambiente local
- [ ] Indice unico para duplicidade previsto no script

### Testes pendentes
- [ ] Confirmar no banco que o indice `UX_LancamentoFinanceiro_Competencia_Descricao_Tipo` existe
- [ ] Testar duplicidade pelo sistema e confirmar bloqueio no banco

## 3. Entidade Principal

### Requisitos do desafio
- `Id`
- `Descricao`
- `Tipo`
- `ValorOriginal`
- `PercentualTaxa`
- `PercentualDesconto`
- `ValorCalculado`
- `DataLancamento`
- `DataCriacao`
- `DataPagamento`
- `DataCancelamento`
- `Competencia`
- `Status`

### Status
- [x] Entidade `LancamentoFinanceiro` criada
- [x] Enums para `TipoLancamento` e `StatusLancamento` criados
- [x] Campos do modelo cobertos no C#
- [x] Validar todos os campos no banco e na tela final

## 4. Regras de Negocio

### Requisitos do desafio
- credito soma no saldo
- debito subtrai do saldo
- taxa e desconto sao mutuamente exclusivos
- taxa somente para debito
- desconto somente para credito
- ambos obrigatorios em seus respectivos tipos
- `DataPagamento` e `DataCancelamento` apenas nos seus processos
- demais informacoes obrigatorias na inclusao e edicao
- nao permitir duplicado por `Competencia + Descricao + Tipo`
- ao editar aberto, recalcular `ValorCalculado`
- apenas `Pago` entra no saldo

### Status de desenvolvimento
- [x] Regra de `Credito` com desconto implementada
- [x] Regra de `Debito` com taxa implementada
- [x] Exclusao mutua de taxa e desconto implementada
- [x] Recalculo de `ValorCalculado` na edicao de aberto implementado
- [x] Regra de saldo considerando apenas `Pago` implementada
- [x] Bloqueio de edicao para status diferente de `Aberto`
- [x] Pagamento e cancelamento separados por processo
- [-] Validacoes amigaveis ainda precisam ser confirmadas em todos os cenarios

### Testes obrigatorios ainda faltando
- [x] Validar `Credito` com desconto obrigatorio
- [x] Validar `Debito` com taxa obrigatoria
- [x] Tentar `Credito` com taxa e verificar mensagem amigavel
- [x] Tentar `Debito` com desconto e verificar mensagem amigavel
- [x] Tentar cadastrar duplicado e verificar bloqueio
- [x] Confirmar que `Pago` soma no saldo
- [x] Confirmar que `Debito Pago` subtrai do saldo
- [x] Confirmar que `Cancelado` nao entra no saldo

## 5. Cadastro do Lancamento

### Requisitos do desafio
- conter todos os campos obrigatorios conforme as regras

### Status de desenvolvimento
- [x] Pagina de cadastro criada
- [x] Formulario abre sem erro
- [x] Payload do formulario corrigido
- [x] Cadastro valido salvo com sucesso no banco
- [x] Tratamento de erro amigavel ainda precisa ser validado nos cenarios invalidos

### Testes obrigatorios ainda faltando
- [ ] Cadastrar um `Credito` valido
- [ ] Cadastrar um `Debito` valido
- [ ] Confirmar mensagem de sucesso
- [ ] Confirmar mensagem de erro em regras invalidas

## 6. Listagem de Lancamentos

### Requisitos do desafio
- apresentar os lancamentos cadastrados
- exibir o saldo em um totalizador

### Status de desenvolvimento
- [x] Pagina inicial lista os lancamentos
- [x] Totalizador de saldo existe
- [x] Filtro por competencia existe
- [x] Status, tipo e valores aparecem na tela
- [x] Exportacao por competencia foi ligada na listagem

### Testes obrigatorios ainda faltando
- [ ] Confirmar que todos os lancamentos cadastrados aparecem
- [ ] Confirmar que o filtro por competencia funciona
- [ ] Confirmar que o saldo bate com o banco

## 7. Edicao do Lancamento

### Requisitos do desafio
- permitida apenas para `Aberto`
- permitir editar todos os campos, exceto:
  - `Status`
  - `DataPagamento`
  - `DataCancelamento`
  - `DataCriacao`

### Status de desenvolvimento
- [x] Tela de edicao criada
- [x] Bloqueio para itens nao `Aberto` implementado
- [x] Recalculo de `ValorCalculado` implementado
- [-] Confirmar que os campos proibidos realmente nao sao editaveis pelo fluxo

### Testes obrigatorios ainda faltando
- [ ] Editar um lancamento `Aberto`
- [ ] Confirmar persistencia no banco
- [ ] Tentar editar um `Pago`
- [ ] Tentar editar um `Cancelado`
- [ ] Confirmar que `Status`, `DataPagamento`, `DataCancelamento` e `DataCriacao` nao sao alterados

## 8. Pagamento do Lancamento

### Requisitos do desafio
- apenas alteracao logica de status
- `DataPagamento` informada nesse processo

### Status de desenvolvimento
- [x] Fluxo de pagamento criado
- [x] Atualizacao de status para `Pago` implementada
- [x] Gravacao de `DataPagamento` implementada

### Testes obrigatorios ainda faltando
- [ ] Pagar um lancamento `Aberto`
- [ ] Confirmar `Status = Pago`
- [ ] Confirmar `DataPagamento` preenchida
- [ ] Confirmar reflexo no saldo

## 9. Cancelamento do Lancamento

### Requisitos do desafio
- apenas alteracao logica de status
- `DataCancelamento` informada nesse processo

### Status de desenvolvimento
- [x] Fluxo de cancelamento criado
- [x] Atualizacao de status para `Cancelado` implementada
- [x] Gravacao de `DataCancelamento` implementada

### Testes obrigatorios ainda faltando
- [ ] Cancelar um lancamento `Aberto`
- [ ] Confirmar `Status = Cancelado`
- [ ] Confirmar `DataCancelamento` preenchida
- [ ] Confirmar que nao entra no saldo

## 10. Exportacao / Impressao

### Requisitos do desafio
- exportar ou imprimir filtrando por uma competencia
- formato aceito: `PDF`, `Excel`, `CSV` ou livre escolha

### Status de desenvolvimento
- [x] Escolha de formato definida como `CSV`
- [x] Exportacao por competencia implementada
- [-] Validar conteudo e formato final do arquivo

### Testes obrigatorios ainda faltando
- [ ] Exportar uma competencia com dados
- [ ] Exportar uma competencia sem dados
- [ ] Abrir o CSV e validar colunas e linhas

## 11. Tratamento de Erros e UX

### Requisitos derivados para entrega
- evitar erro 500 em regra de negocio
- mostrar mensagens compreensiveis para demonstracao

### Status de desenvolvimento
- [x] `UseDeveloperExceptionPage()` configurado para diagnostico local
- [x] Tratamento de `DomainException` existe no cadastro
- [-] Tratamento amigavel ainda precisa ser padronizado em cadastro, edicao, pagamento e cancelamento
- [-] Front-end esta mais amigavel, mas ainda simples

### Testes obrigatorios ainda faltando
- [ ] Forcar erros de regra e ver mensagem amigavel
- [ ] Confirmar que formularios invalidos nao derrubam a aplicacao

## 12. Front-end e Demonstracao

### Requisitos praticos para boa entrega
- tela inicial clara
- formulario compreensivel
- navegacao simples

### Status
- [x] Tela inicial melhorada
- [x] Formulario de cadastro simplificado
- [x] Estilos basicos aplicados
- [-] Confirmar se assets estaticos estao carregando corretamente no ambiente final

### Testes ainda faltando
- [ ] Confirmar carregamento de CSS
- [ ] Confirmar carregamento de JS
- [ ] Verificar se o sistema fica apresentavel para demonstracao

## 13. Revisao Final de Entrega

### Falta fazer antes de considerar pronto
- [ ] Confirmar banco, tabela e indice no ambiente final
- [ ] Executar os testes manuais dos fluxos obrigatorios
- [ ] Revisar textos e mensagens das telas
- [ ] Limpar arquivos locais desnecessarios do commit
- [ ] Validar exportacao CSV
- [ ] Fazer uma demonstracao completa de ponta a ponta

## 14. Prioridade Recomendada Agora

1. Confirmar banco, tabela e indice unico
2. Testar `Debito` valido
3. Testar erros amigaveis no cadastro
4. Testar edicao
5. Testar pagamento e cancelamento
6. Validar saldo
7. Validar exportacao CSV
8. Revisao final da demonstracao
