using CodeBehind.Enums;
using System.ComponentModel.DataAnnotations;

namespace CodeBehind.Models;

public class LancamentoInputModel
{
    [Required(ErrorMessage = "Informe a descricao.")]
    [StringLength(200, ErrorMessage = "A descricao deve ter no maximo 200 caracteres.")]
    public string Descricao { get; set; } = string.Empty;

    [Required(ErrorMessage = "Selecione o tipo do lancamento.")]
    public TipoLancamento Tipo { get; set; }

    [Range(typeof(decimal), "0,01", "999999999", ErrorMessage = "O lancamento nao foi cadastrado porque o valor original deve ser maior que zero.")]
    public decimal ValorOriginal { get; set; }

    [Range(typeof(decimal), "0", "100", ErrorMessage = "A taxa deve estar entre 0 e 100.")]
    public decimal? PercentualTaxa { get; set; }

    [Range(typeof(decimal), "0", "100", ErrorMessage = "O desconto deve estar entre 0 e 100.")]
    public decimal? PercentualDesconto { get; set; }

    [Required(ErrorMessage = "Informe a data do lancamento.")]
    public DateTime DataLancamento { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "Informe a competencia.")]
    [RegularExpression(@"^\d{4}\-(0[1-9]|1[0-2])$", ErrorMessage = "Use o formato AAAA-MM.")]
    public string Competencia { get; set; } = string.Empty;
}
