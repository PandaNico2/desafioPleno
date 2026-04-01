using CodeBehind.Enums;
using CodeBehind.Models;
using CodeBehind.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace frontend.Pages.Lancamentos;

public class EditarModel : PageModel
{
    private readonly LancamentoService _service;

    public EditarModel(LancamentoService service)
    {
        _service = service;
    }

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    [BindProperty]
    public LancamentoInputModel Input { get; set; } = new();

    public IEnumerable<SelectListItem> Tipos { get; } = new[]
    {
        new SelectListItem("Credito", ((int)TipoLancamento.Credito).ToString()),
        new SelectListItem("Debito", ((int)TipoLancamento.Debito).ToString())
    };

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var item = await _service.ObterPorIdAsync(Id, cancellationToken);
        if (item is null)
        {
            TempData["Erro"] = "Lancamento nao encontrado.";
            return RedirectToPage("/Index");
        }

        if (item.Status != StatusLancamento.Aberto)
        {
            TempData["Erro"] = "Somente lancamentos em aberto podem ser editados.";
            return RedirectToPage("/Index");
        }

        Input = new LancamentoInputModel
        {
            Descricao = item.Descricao,
            Tipo = item.Tipo,
            ValorOriginal = item.ValorOriginal,
            PercentualTaxa = item.PercentualTaxa,
            PercentualDesconto = item.PercentualDesconto,
            DataLancamento = item.DataLancamento,
            Competencia = item.Competencia
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            await _service.AtualizarAsync(Id, Input, cancellationToken);
            TempData["Mensagem"] = "Lancamento atualizado com sucesso.";
            return RedirectToPage("/Index");
        }
        catch (DomainException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
    }
}
