using CodeBehind.Enums;
using CodeBehind.Models;
using CodeBehind.Services;
using Microsoft.Data.SqlClient;
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
            AddFriendlyValidationSummary();
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
        catch (SqlException ex)
        {
            ModelState.AddModelError(string.Empty, $"Erro ao acessar o banco de dados: {ex.Message}");
            return Page();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"Erro inesperado: {ex.Message}");
            return Page();
        }
    }

    private void AddFriendlyValidationSummary()
    {
        ReplaceEnglishNumberMessage("Input.ValorOriginal", "O valor original deve ser um numero valido.");
        ReplaceEnglishNumberMessage("Input.PercentualTaxa", "A taxa deve ser um numero valido.");
        ReplaceEnglishNumberMessage("Input.PercentualDesconto", "O desconto deve ser um numero valido.");

        if (ModelState.ContainsKey("Input.ValorOriginal") && ModelState["Input.ValorOriginal"]?.Errors.Count > 0)
        {
            ModelState.AddModelError(string.Empty, "O lancamento nao foi atualizado porque o valor original deve ser maior que zero.");
        }
    }

    private void ReplaceEnglishNumberMessage(string key, string message)
    {
        if (!ModelState.ContainsKey(key) || ModelState[key] is null)
        {
            return;
        }

        var entry = ModelState[key]!;
        if (entry.Errors.Count == 0)
        {
            return;
        }

        var hasEnglishMessage = entry.Errors.Any(error =>
            error.ErrorMessage.Contains("must be a number", StringComparison.OrdinalIgnoreCase));

        if (!hasEnglishMessage)
        {
            return;
        }

        entry.Errors.Clear();
        entry.Errors.Add(message);
    }
}
