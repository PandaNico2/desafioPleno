using CodeBehind.Enums;
using CodeBehind.Models;
using CodeBehind.Services;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace frontend.Pages.Lancamentos;

public class CriarModel : PageModel
{
    private readonly LancamentoService _service;

    public CriarModel(LancamentoService service)
    {
        _service = service;
    }

    [BindProperty]
    public LancamentoInputModel Input { get; set; } = new()
    {
        DataLancamento = DateTime.Now,
        Competencia = DateTime.Now.ToString("yyyy-MM")
    };

    public IEnumerable<SelectListItem> Tipos { get; } = new[]
    {
        new SelectListItem("Credito", ((int)TipoLancamento.Credito).ToString()),
        new SelectListItem("Debito", ((int)TipoLancamento.Debito).ToString())
    };

    public string? ValorPrevisto { get; private set; }

    public void OnGet()
    {
    }

    public IActionResult OnPostPreview()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            ValorPrevisto = _service.CalcularValor(Input).ToString("C");
            return Page();
        }
        catch (DomainException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            await _service.CriarAsync(Input, cancellationToken);
            TempData["Mensagem"] = "Lancamento criado com sucesso.";
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
}
