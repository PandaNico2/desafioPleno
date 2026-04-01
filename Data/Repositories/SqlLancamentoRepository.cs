using CodeBehind.Entities;
using CodeBehind.Enums;
using CodeBehind.Interfaces;
using CodeBehind.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Data.Repositories;

public class SqlLancamentoRepository : ILancamentoRepository
{
    private readonly string _connectionString;

    public SqlLancamentoRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<int> InserirAsync(LancamentoFinanceiro lancamento, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO dbo.LancamentoFinanceiro
            (
                Descricao,
                Tipo,
                ValorOriginal,
                PercentualTaxa,
                PercentualDesconto,
                ValorCalculado,
                DataLancamento,
                DataCriacao,
                DataPagamento,
                DataCancelamento,
                Competencia,
                Status
            )
            OUTPUT INSERTED.Id
            VALUES
            (
                @Descricao,
                @Tipo,
                @ValorOriginal,
                @PercentualTaxa,
                @PercentualDesconto,
                @ValorCalculado,
                @DataLancamento,
                @DataCriacao,
                @DataPagamento,
                @DataCancelamento,
                @Competencia,
                @Status
            );
            """;

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);
        PreencherParametros(command, lancamento);

        try
        {
            var result = await command.ExecuteScalarAsync(cancellationToken);
            return Convert.ToInt32(result);
        }
        catch (SqlException ex) when (ex.Number is 2601 or 2627)
        {
            throw new DomainException("Ja existe um lancamento com a mesma competencia, descricao e tipo.");
        }
    }

    public async Task AtualizarAsync(LancamentoFinanceiro lancamento, CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE dbo.LancamentoFinanceiro
            SET
                Descricao = @Descricao,
                Tipo = @Tipo,
                ValorOriginal = @ValorOriginal,
                PercentualTaxa = @PercentualTaxa,
                PercentualDesconto = @PercentualDesconto,
                ValorCalculado = @ValorCalculado,
                DataLancamento = @DataLancamento,
                Competencia = @Competencia
            WHERE Id = @Id;
            """;

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);
        PreencherParametros(command, lancamento);

        try
        {
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
        catch (SqlException ex) when (ex.Number is 2601 or 2627)
        {
            throw new DomainException("Ja existe um lancamento com a mesma competencia, descricao e tipo.");
        }
    }

    public async Task AtualizarStatusAsync(int id, StatusLancamento status, DateTime dataProcessamento, CancellationToken cancellationToken = default)
    {
        var campoData = status switch
        {
            StatusLancamento.Pago => "DataPagamento",
            StatusLancamento.Cancelado => "DataCancelamento",
            _ => throw new DomainException("Status de processamento invalido.")
        };

        var limparCampo = status == StatusLancamento.Pago ? "DataCancelamento = NULL" : "DataPagamento = NULL";
        var sql = $"""
            UPDATE dbo.LancamentoFinanceiro
            SET
                Status = @Status,
                {campoData} = @DataProcessamento,
                {limparCampo}
            WHERE Id = @Id;
            """;

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);
        command.Parameters.AddWithValue("@Status", (int)status);
        command.Parameters.AddWithValue("@DataProcessamento", dataProcessamento);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<bool> ExisteDuplicadoAsync(string competencia, string descricao, TipoLancamento tipo, int? ignorarId = null, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT COUNT(1)
            FROM dbo.LancamentoFinanceiro
            WHERE Competencia = @Competencia
              AND Descricao = @Descricao
              AND Tipo = @Tipo
              AND (@IgnorarId IS NULL OR Id <> @IgnorarId);
            """;

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Competencia", competencia);
        command.Parameters.AddWithValue("@Descricao", descricao);
        command.Parameters.AddWithValue("@Tipo", (int)tipo);
        command.Parameters.AddWithValue("@IgnorarId", (object?)ignorarId ?? DBNull.Value);
        var total = Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken));
        return total > 0;
    }

    public async Task<IReadOnlyList<LancamentoFinanceiro>> ListarAsync(string? competencia = null, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                Id,
                Descricao,
                Tipo,
                ValorOriginal,
                PercentualTaxa,
                PercentualDesconto,
                ValorCalculado,
                DataLancamento,
                DataCriacao,
                DataPagamento,
                DataCancelamento,
                Competencia,
                Status
            FROM dbo.LancamentoFinanceiro
            WHERE (@Competencia IS NULL OR Competencia = @Competencia)
            ORDER BY DataLancamento DESC, Id DESC;
            """;

        var itens = new List<LancamentoFinanceiro>();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Competencia", (object?)competencia ?? DBNull.Value);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            itens.Add(Mapear(reader));
        }

        return itens;
    }

    public async Task<LancamentoFinanceiro?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                Id,
                Descricao,
                Tipo,
                ValorOriginal,
                PercentualTaxa,
                PercentualDesconto,
                ValorCalculado,
                DataLancamento,
                DataCriacao,
                DataPagamento,
                DataCancelamento,
                Competencia,
                Status
            FROM dbo.LancamentoFinanceiro
            WHERE Id = @Id;
            """;

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);

        await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow, cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return Mapear(reader);
    }

    public async Task<decimal> ObterSaldoAsync(string? competencia = null, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                COALESCE(SUM(
                    CASE
                        WHEN Tipo = @TipoCredito THEN ValorCalculado
                        ELSE -ValorCalculado
                    END
                ), 0)
            FROM dbo.LancamentoFinanceiro
            WHERE Status = @StatusPago
              AND (@Competencia IS NULL OR Competencia = @Competencia);
            """;

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@TipoCredito", (int)TipoLancamento.Credito);
        command.Parameters.AddWithValue("@StatusPago", (int)StatusLancamento.Pago);
        command.Parameters.AddWithValue("@Competencia", (object?)competencia ?? DBNull.Value);
        var result = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToDecimal(result);
    }

    private static LancamentoFinanceiro Mapear(SqlDataReader reader)
    {
        return new LancamentoFinanceiro
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            Descricao = reader.GetString(reader.GetOrdinal("Descricao")),
            Tipo = (TipoLancamento)reader.GetInt32(reader.GetOrdinal("Tipo")),
            ValorOriginal = reader.GetDecimal(reader.GetOrdinal("ValorOriginal")),
            PercentualTaxa = reader.IsDBNull(reader.GetOrdinal("PercentualTaxa"))
                ? null
                : reader.GetDecimal(reader.GetOrdinal("PercentualTaxa")),
            PercentualDesconto = reader.IsDBNull(reader.GetOrdinal("PercentualDesconto"))
                ? null
                : reader.GetDecimal(reader.GetOrdinal("PercentualDesconto")),
            ValorCalculado = reader.GetDecimal(reader.GetOrdinal("ValorCalculado")),
            DataLancamento = reader.GetDateTime(reader.GetOrdinal("DataLancamento")),
            DataCriacao = reader.GetDateTime(reader.GetOrdinal("DataCriacao")),
            DataPagamento = reader.IsDBNull(reader.GetOrdinal("DataPagamento"))
                ? null
                : reader.GetDateTime(reader.GetOrdinal("DataPagamento")),
            DataCancelamento = reader.IsDBNull(reader.GetOrdinal("DataCancelamento"))
                ? null
                : reader.GetDateTime(reader.GetOrdinal("DataCancelamento")),
            Competencia = reader.GetString(reader.GetOrdinal("Competencia")).Trim(),
            Status = (StatusLancamento)reader.GetInt32(reader.GetOrdinal("Status"))
        };
    }

    private static void PreencherParametros(SqlCommand command, LancamentoFinanceiro lancamento)
    {
        command.Parameters.AddWithValue("@Id", lancamento.Id);
        command.Parameters.AddWithValue("@Descricao", lancamento.Descricao);
        command.Parameters.AddWithValue("@Tipo", (int)lancamento.Tipo);
        command.Parameters.AddWithValue("@ValorOriginal", lancamento.ValorOriginal);
        command.Parameters.AddWithValue("@PercentualTaxa", (object?)lancamento.PercentualTaxa ?? DBNull.Value);
        command.Parameters.AddWithValue("@PercentualDesconto", (object?)lancamento.PercentualDesconto ?? DBNull.Value);
        command.Parameters.AddWithValue("@ValorCalculado", lancamento.ValorCalculado);
        command.Parameters.AddWithValue("@DataLancamento", lancamento.DataLancamento);
        command.Parameters.AddWithValue("@DataCriacao", lancamento.DataCriacao);
        command.Parameters.AddWithValue("@DataPagamento", (object?)lancamento.DataPagamento ?? DBNull.Value);
        command.Parameters.AddWithValue("@DataCancelamento", (object?)lancamento.DataCancelamento ?? DBNull.Value);
        command.Parameters.AddWithValue("@Competencia", lancamento.Competencia);
        command.Parameters.AddWithValue("@Status", (int)lancamento.Status);
    }
}
