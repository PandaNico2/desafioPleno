using CodeBehind.Models;
using CodeBehind.Services;
using CodeBehind.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace frontend.Pages.Lancamentos;

public class PagarModel : PageModel
{
    private readonly LancamentoService _service;

    public PagarModel(LancamentoService service)
    {
        _service = service;
    }

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    [BindProperty]
    public DateTime DataProcessamento { get; set; } = DateTime.Now;

    public string Descricao { get; private set; } = string.Empty;
    public string AcaoTitulo { get; private set; } = "Registrar pagamento";
    public string AcaoBotao { get; private set; } = "Confirmar pagamento";
    public string AcaoTexto { get; private set; } = "pagos";

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var item = await _service.ObterPorIdAsync(Id, cancellationToken);
        if (item is null)
        {
            TempData["Erro"] = "Lancamento nao encontrado.";
            return RedirectToPage("/Index");
        }

        Descricao = item.Descricao;
        SetTexts(item.Tipo);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _service.RegistrarPagamentoAsync(Id, DataProcessamento, cancellationToken);
            TempData["Mensagem"] = "Pagamento registrado com sucesso.";
            return RedirectToPage("/Index");
        }
        catch (DomainException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            var item = await _service.ObterPorIdAsync(Id, cancellationToken);
            Descricao = item?.Descricao ?? string.Empty;
            if (item is not null)
            {
                SetTexts(item.Tipo);
            }
            return Page();
        }
        catch (SqlException ex)
        {
            ModelState.AddModelError(string.Empty, $"Erro ao acessar o banco de dados: {ex.Message}");
            var item = await _service.ObterPorIdAsync(Id, cancellationToken);
            Descricao = item?.Descricao ?? string.Empty;
            if (item is not null)
            {
                SetTexts(item.Tipo);
            }
            return Page();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"Erro inesperado: {ex.Message}");
            var item = await _service.ObterPorIdAsync(Id, cancellationToken);
            Descricao = item?.Descricao ?? string.Empty;
            if (item is not null)
            {
                SetTexts(item.Tipo);
            }
            return Page();
        }
    }

    private void SetTexts(TipoLancamento tipo)
    {
        var isCredito = tipo == TipoLancamento.Credito;
        AcaoTitulo = isCredito ? "Registrar recebimento" : "Registrar pagamento";
        AcaoBotao = isCredito ? "Confirmar recebimento" : "Confirmar pagamento";
        AcaoTexto = isCredito ? "recebidos" : "pagos";
    }
}
