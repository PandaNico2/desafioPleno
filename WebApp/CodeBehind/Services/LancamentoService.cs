using CodeBehind.Entities;
using CodeBehind.Enums;
using CodeBehind.Interfaces;
using CodeBehind.Models;

namespace CodeBehind.Services;

public class LancamentoService
{
    private readonly ILancamentoRepository _repository;

    public LancamentoService(ILancamentoRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<LancamentoFinanceiro>> ListarAsync(string? competencia = null, CancellationToken cancellationToken = default)
        => _repository.ListarAsync(NormalizarCompetenciaOpcional(competencia), cancellationToken);

    public Task<decimal> ObterSaldoAsync(string? competencia = null, CancellationToken cancellationToken = default)
        => _repository.ObterSaldoAsync(NormalizarCompetenciaOpcional(competencia), cancellationToken);

    public Task<LancamentoFinanceiro?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
        => _repository.ObterPorIdAsync(id, cancellationToken);

    public async Task<int> CriarAsync(LancamentoInputModel input, CancellationToken cancellationToken = default)
    {
        ValidarEntrada(input);

        var descricao = input.Descricao.Trim();
        var competencia = NormalizarCompetencia(input.Competencia);
        if (await _repository.ExisteDuplicadoAsync(competencia, descricao, input.Tipo, null, cancellationToken))
        {
            throw new DomainException("Ja existe um lancamento com a mesma competencia, descricao e tipo.");
        }

        var lancamento = new LancamentoFinanceiro
        {
            Descricao = descricao,
            Tipo = input.Tipo,
            ValorOriginal = input.ValorOriginal,
            PercentualTaxa = input.Tipo == TipoLancamento.Debito ? input.PercentualTaxa : null,
            PercentualDesconto = input.Tipo == TipoLancamento.Credito ? input.PercentualDesconto : null,
            ValorCalculado = CalcularValor(input),
            DataLancamento = input.DataLancamento,
            DataCriacao = DateTime.Now,
            Competencia = competencia,
            Status = StatusLancamento.Aberto
        };

        return await _repository.InserirAsync(lancamento, cancellationToken);
    }

    public async Task AtualizarAsync(int id, LancamentoInputModel input, CancellationToken cancellationToken = default)
    {
        ValidarEntrada(input);

        var atual = await _repository.ObterPorIdAsync(id, cancellationToken)
            ?? throw new DomainException("Lancamento nao encontrado.");

        if (atual.Status != StatusLancamento.Aberto)
        {
            throw new DomainException("Somente lancamentos em aberto podem ser editados.");
        }

        var descricao = input.Descricao.Trim();
        var competencia = NormalizarCompetencia(input.Competencia);
        if (await _repository.ExisteDuplicadoAsync(competencia, descricao, input.Tipo, id, cancellationToken))
        {
            throw new DomainException("Ja existe um lancamento com a mesma competencia, descricao e tipo.");
        }

        atual.Descricao = descricao;
        atual.Tipo = input.Tipo;
        atual.ValorOriginal = input.ValorOriginal;
        atual.PercentualTaxa = input.Tipo == TipoLancamento.Debito ? input.PercentualTaxa : null;
        atual.PercentualDesconto = input.Tipo == TipoLancamento.Credito ? input.PercentualDesconto : null;
        atual.ValorCalculado = CalcularValor(input);
        atual.DataLancamento = input.DataLancamento;
        atual.Competencia = competencia;

        await _repository.AtualizarAsync(atual, cancellationToken);
    }

    public async Task RegistrarPagamentoAsync(int id, DateTime dataPagamento, CancellationToken cancellationToken = default)
    {
        var atual = await _repository.ObterPorIdAsync(id, cancellationToken)
            ?? throw new DomainException("Lancamento nao encontrado.");

        if (atual.Status != StatusLancamento.Aberto)
        {
            throw new DomainException("Somente lancamentos em aberto podem ser pagos.");
        }

        await _repository.AtualizarStatusAsync(id, StatusLancamento.Pago, dataPagamento, cancellationToken);
    }

    public async Task RegistrarCancelamentoAsync(int id, DateTime dataCancelamento, CancellationToken cancellationToken = default)
    {
        var atual = await _repository.ObterPorIdAsync(id, cancellationToken)
            ?? throw new DomainException("Lancamento nao encontrado.");

        if (atual.Status != StatusLancamento.Aberto)
        {
            throw new DomainException("Somente lancamentos em aberto podem ser cancelados.");
        }

        await _repository.AtualizarStatusAsync(id, StatusLancamento.Cancelado, dataCancelamento, cancellationToken);
    }

    public decimal CalcularValor(LancamentoInputModel input)
    {
        var percentual = input.Tipo == TipoLancamento.Debito
            ? input.PercentualTaxa!.Value
            : input.PercentualDesconto!.Value;

        var ajuste = input.ValorOriginal * (percentual / 100m);

        return input.Tipo == TipoLancamento.Debito
            ? input.ValorOriginal + ajuste
            : input.ValorOriginal - ajuste;
    }

    private static void ValidarEntrada(LancamentoInputModel input)
    {
        if (string.IsNullOrWhiteSpace(input.Descricao))
        {
            throw new DomainException("Informe a descricao.");
        }

        if (string.IsNullOrWhiteSpace(input.Competencia))
        {
            throw new DomainException("Informe a competencia.");
        }

        if (input.Tipo == TipoLancamento.Debito)
        {
            if (!input.PercentualTaxa.HasValue)
            {
                throw new DomainException("Debitos exigem percentual de taxa.");
            }

            if (input.PercentualDesconto.HasValue)
            {
                throw new DomainException("Debitos nao podem receber desconto.");
            }
        }
        else
        {
            if (!input.PercentualDesconto.HasValue)
            {
                throw new DomainException("Creditos exigem percentual de desconto.");
            }

            if (input.PercentualTaxa.HasValue)
            {
                throw new DomainException("Creditos nao podem receber taxa.");
            }
        }
    }

    private static string? NormalizarCompetenciaOpcional(string? competencia)
        => string.IsNullOrWhiteSpace(competencia) ? null : NormalizarCompetencia(competencia);

    private static string NormalizarCompetencia(string competencia)
    {
        var valor = competencia.Trim();
        if (valor.Length != 7 || valor[4] != '-')
        {
            throw new DomainException("A competencia deve estar no formato AAAA-MM.");
        }

        return valor;
    }
}
