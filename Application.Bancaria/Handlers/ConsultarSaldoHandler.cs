using API.Bancaria.Query;
using API.Bancaria.Repository;
using API.Bancaria.Response;
using Domain.Bancaria.Exceptions;
using MediatR;

namespace Application.Bancaria.Handlers;
public class ConsultarSaldoHandler : IRequestHandler<ConsultarSaldoQuery, SaldoContaCorrenteResponse>
{
    private readonly IContaCorrenteRepository _contaCorrenteRepository;
    private readonly IMovimentoRepository _movimentoRepository;

    public ConsultarSaldoHandler(
        IContaCorrenteRepository contaCorrenteRepository,
        IMovimentoRepository movimentoRepository)
    {
        _contaCorrenteRepository = contaCorrenteRepository;
        _movimentoRepository = movimentoRepository;
    }

    public async Task<SaldoContaCorrenteResponse> Handle(ConsultarSaldoQuery request, CancellationToken cancellationToken)
    {
        // Validar conta corrente
        var conta = await _contaCorrenteRepository.ObterPorIdAsync(request.IdContaCorrente);
        if (conta == null)
        {
            throw DomainException.ContaInvalida();
        }

        // Validar se a conta está ativa
        if (!conta.EstaAtiva())
        {
            throw DomainException.ContaInativa();
        }

        // Obter movimentos da conta
        var movimentos = await _movimentoRepository.ObterPorContaCorrenteAsync(request.IdContaCorrente);

        // Calcular saldo
        decimal somaDosCreditos = movimentos
            .Where(m => m.TipoMovimentacao == "C")
            .Sum(m => m.Valor);

        decimal somaDosDebitos = movimentos
            .Where(m => m.TipoMovimentacao == "D")
            .Sum(m => m.Valor);

        decimal saldo = somaDosCreditos - somaDosDebitos;

        // Criar resposta
        return new SaldoContaCorrenteResponse
        {
            NumeroConta = conta.Numero,
            NomeTitular = conta.Nome,
            DataConsulta = DateTime.Now,
            Saldo = saldo
        };
    }
}
