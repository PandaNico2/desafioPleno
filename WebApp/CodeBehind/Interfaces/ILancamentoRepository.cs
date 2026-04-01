using CodeBehind.Entities;
using CodeBehind.Enums;

namespace CodeBehind.Interfaces;

public interface ILancamentoRepository
{
    Task<int> InserirAsync(LancamentoFinanceiro lancamento, CancellationToken cancellationToken = default);
    Task AtualizarAsync(LancamentoFinanceiro lancamento, CancellationToken cancellationToken = default);
    Task AtualizarStatusAsync(int id, StatusLancamento status, DateTime dataProcessamento, CancellationToken cancellationToken = default);
    Task<bool> ExisteDuplicadoAsync(string competencia, string descricao, TipoLancamento tipo, int? ignorarId = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LancamentoFinanceiro>> ListarAsync(string? competencia = null, CancellationToken cancellationToken = default);
    Task<LancamentoFinanceiro?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<decimal> ObterSaldoAsync(string? competencia = null, CancellationToken cancellationToken = default);
}
