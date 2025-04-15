using API.Bancaria.Response;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace API.Bancaria.Command;

public class RealizarMovimentacaoCommand : IRequest<MovimentacaoResponse>
{
    [Required]
    public string ChaveIdempotencia { get; set; }

    [Required]
    public string IdContaCorrente { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero")]
    public decimal Valor { get; set; }

    [Required]
    [RegularExpression("^[CD]$", ErrorMessage = "Tipo de movimento deve ser C (Crédito) ou D (Débito)")]
    public string TipoMovimentacao { get; set; }
}
