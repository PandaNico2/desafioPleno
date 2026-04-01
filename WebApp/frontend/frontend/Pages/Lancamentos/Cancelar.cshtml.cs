using CodeBehind.Models;
using CodeBehind.Services;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace frontend.Pages.Lancamentos;

public class CancelarModel : PageModel
{
    private readonly LancamentoService _service;

    public CancelarModel(LancamentoService service)
    {
        _service = service;
    }

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    [BindProperty]
    public DateTime DataProcessamento { get; set; } = DateTime.Now;

    public string Descricao { get; private set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var item = await _service.ObterPorIdAsync(Id, cancellationToken);
        if (item is null)
        {
            TempData["Erro"] = "Lancamento nao encontrado.";
            return RedirectToPage("/Index");
        }

        Descricao = item.Descricao;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _service.RegistrarCancelamentoAsync(Id, DataProcessamento, cancellationToken);
            TempData["Mensagem"] = "Cancelamento registrado com sucesso.";
            return RedirectToPage("/Index");
        }
        catch (DomainException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            var item = await _service.ObterPorIdAsync(Id, cancellationToken);
            Descricao = item?.Descricao ?? string.Empty;
            return Page();
        }
        catch (SqlException ex)
        {
            ModelState.AddModelError(string.Empty, $"Erro ao acessar o banco de dados: {ex.Message}");
            var item = await _service.ObterPorIdAsync(Id, cancellationToken);
            Descricao = item?.Descricao ?? string.Empty;
            return Page();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"Erro inesperado: {ex.Message}");
            var item = await _service.ObterPorIdAsync(Id, cancellationToken);
            Descricao = item?.Descricao ?? string.Empty;
            return Page();
        }
    }
}
