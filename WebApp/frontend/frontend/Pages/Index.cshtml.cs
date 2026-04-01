using CodeBehind.Entities;
using CodeBehind.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

    public IReadOnlyList<LancamentoFinanceiro> Lancamentos { get; private set; } = [];
    public decimal Saldo { get; private set; }
    public string? Mensagem => TempData["Mensagem"] as string;
    public string? Erro => TempData["Erro"] as string;

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Lancamentos = await _service.ListarAsync(Competencia, cancellationToken);
        Saldo = await _service.ObterSaldoAsync(Competencia, cancellationToken);
    }

    public async Task<IActionResult> OnGetExportarAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(Competencia))
        {
            TempData["Erro"] = "Informe uma competencia para exportar.";
            return RedirectToPage("/Index");
        }

        var lancamentos = await _service.ListarAsync(Competencia, cancellationToken);
        var builder = new StringBuilder();
        builder.AppendLine("Id,Descricao,Competencia,Tipo,Status,ValorOriginal,ValorCalculado,DataLancamento,DataPagamento,DataCancelamento");

        foreach (var item in lancamentos)
        {
            builder.AppendLine(string.Join(",",
                item.Id,
                EscapeCsv(item.Descricao),
                item.Competencia,
                item.Tipo,
                item.Status,
                item.ValorOriginal.ToString("0.00"),
                item.ValorCalculado.ToString("0.00"),
                item.DataLancamento.ToString("yyyy-MM-dd HH:mm:ss"),
                item.DataPagamento?.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty,
                item.DataCancelamento?.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty));
        }

        var bytes = Encoding.UTF8.GetBytes(builder.ToString());
        return File(bytes, "text/csv", $"lancamentos-{Competencia}.csv");
    }

    private static string EscapeCsv(string value)
    {
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }

        return value;
    }
}
