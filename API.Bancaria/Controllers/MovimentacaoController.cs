using API.Bancaria.Command;
using API.Bancaria.Query;
using Domain.Bancaria.Exceptions;
using Domain.Bancaria.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Bancaria.Controllers;
[Route("api/[controller]")]
[ApiController]
public class MovimentacaoController : ControllerBase
{
    private readonly IMediator _mediator;

    public MovimentacaoController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Realiza uma movimentação na conta corrente
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> RealizarMovimentacao([FromBody] RealizarMovimentacaoCommand command)
    {
        try
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
        catch (DomainException ex)
        {
            return BadRequest(new ErrorResponse
            {
                Mensagem = ex.Message,
                Tipo = ex.Tipo
            });
        }
    }

    /// <summary>
    /// Consulta o saldo de uma conta corrente
    /// </summary>
    [HttpGet("{idContaCorrente}")]
    public async Task<IActionResult> ConsultarSaldo(string idContaCorrente)
    {
        try
        {
            Console.WriteLine($"Consultando saldo para conta: {idContaCorrente}");

            var query = new ConsultarSaldoQuery { IdContaCorrente = idContaCorrente };
            var response = await _mediator.Send(query);

            Console.WriteLine($"Saldo consultado com sucesso: {response.Saldo}");
            return Ok(response);
        }
        catch (DomainException ex)
        {
            Console.WriteLine($"Erro de domínio: {ex.Message}, Tipo: {ex.Tipo}");
            return BadRequest(new ErrorResponse
            {
                Mensagem = ex.Message,
                Tipo = ex.Tipo
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro não tratado: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return StatusCode(500, new
            {
                Mensagem = "Erro interno do servidor",
                Detalhe = ex.Message
            });
        }
    }

}
