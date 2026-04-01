using CodeBehind.Entities;
using CodeBehind.Enums;
using CodeBehind.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;

namespace frontend.Pages;

public class IndexModel : PageModel
{
    private readonly LancamentoService _service;

    public IndexModel(LancamentoService service)
    {
        _service = service;
    }

    [BindProperty(SupportsGet = true)]
    public string? Competencia { get; set; }

    [BindProperty(SupportsGet = true)]
    public TipoLancamento? Tipo { get; set; }

    [BindProperty(SupportsGet = true)]
    public StatusLancamento? Status { get; set; }

    public IReadOnlyList<LancamentoFinanceiro> Lancamentos { get; private set; } = [];
    public decimal Saldo { get; private set; }
    public string? Mensagem => TempData["Mensagem"] as string;
    public string? Erro => TempData["Erro"] as string;
    public IEnumerable<SelectListItem> Tipos { get; } = new[]
    {
        new SelectListItem("Todos", string.Empty),
        new SelectListItem("Credito", ((int)TipoLancamento.Credito).ToString()),
        new SelectListItem("Debito", ((int)TipoLancamento.Debito).ToString())
    };

    public IEnumerable<SelectListItem> Statuses { get; } = new[]
    {
        new SelectListItem("Todos", string.Empty),
        new SelectListItem("Aberto", ((int)StatusLancamento.Aberto).ToString()),
        new SelectListItem("Pago", ((int)StatusLancamento.Pago).ToString()),
        new SelectListItem("Cancelado", ((int)StatusLancamento.Cancelado).ToString())
    };

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        var lancamentos = await _service.ListarAsync(Competencia, cancellationToken);
        Lancamentos = ApplyFilters(lancamentos);
        Saldo = await _service.ObterSaldoAsync(Competencia, cancellationToken);
    }

    public async Task<IActionResult> OnGetExportarAsync(CancellationToken cancellationToken)
    {
        var lancamentos = ApplyFilters(await _service.ListarAsync(Competencia, cancellationToken));
        if (lancamentos.Count == 0)
        {
            TempData["Erro"] = string.IsNullOrWhiteSpace(Competencia)
                ? "Nao existem lancamentos para exportar."
                : "Nao existem lancamentos para a competencia informada.";
            return RedirectToPage("/Index", new { Competencia, Tipo, Status });
        }

        var builder = new StringBuilder();
        builder.AppendLine("Id;Descricao;Competencia;Tipo;Status;ValorOriginal;ValorCalculado;DataCriacao");

        foreach (var item in lancamentos)
        {
            builder.AppendLine(string.Join(";",
                item.Id,
                EscapeCsv(item.Descricao),
                item.Competencia,
                item.Tipo,
                item.Status,
                item.ValorOriginal.ToString("0.00"),
                item.ValorCalculado.ToString("0.00"),
                EscapeCsv(item.DataCriacao.ToString("dd/MM/yyyy HH:mm:ss"))));
        }

        var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(builder.ToString())).ToArray();
        var nomeArquivo = string.IsNullOrWhiteSpace(Competencia)
            ? "lancamentos-todos.csv"
            : $"lancamentos-{Competencia}.csv";

        return File(bytes, "text/csv; charset=utf-8", nomeArquivo);
    }

    private IReadOnlyList<LancamentoFinanceiro> ApplyFilters(IReadOnlyList<LancamentoFinanceiro> lancamentos)
    {
        IEnumerable<LancamentoFinanceiro> query = lancamentos;

        if (Tipo.HasValue)
        {
            query = query.Where(item => item.Tipo == Tipo.Value);
        }

        if (Status.HasValue)
        {
            query = query.Where(item => item.Status == Status.Value);
        }

        return query.ToList();
    }

    private static string EscapeCsv(string value)
    {
        if (value.Contains(';') || value.Contains('"') || value.Contains('\n'))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }

        return value;
    }
}
