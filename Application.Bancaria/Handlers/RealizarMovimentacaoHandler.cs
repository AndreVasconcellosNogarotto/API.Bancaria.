using API.Bancaria.Command;
using API.Bancaria.Models;
using API.Bancaria.Repository;
using API.Bancaria.Response;
using Domain.Bancaria.Exceptions;
using Domain.Bancaria.Repositories;
using MediatR;
using System.Text.Json;

namespace API.Bancaria.Handlers;

public class RealizarMovimentacaoHandler : IRequestHandler<RealizarMovimentacaoCommand, MovimentacaoResponse>
{
    private readonly IContaCorrenteRepository _contaCorrenteRepository;
    private readonly IMovimentoRepository _movimentoRepository;
    private readonly IIdempotenciaRepository _idempotenciaRepository;

    public RealizarMovimentacaoHandler(
        IContaCorrenteRepository contaCorrenteRepository,
        IMovimentoRepository movimentoRepository,
        IIdempotenciaRepository idempotenciaRepository)
    {
        _contaCorrenteRepository = contaCorrenteRepository;
        _movimentoRepository = movimentoRepository;
        _idempotenciaRepository = idempotenciaRepository;
    }

    public async Task<MovimentacaoResponse> Handle(RealizarMovimentacaoCommand request, CancellationToken cancellationToken)
    {
        // Verificar idempotência
        var idempotencia = await _idempotenciaRepository.ObterPorChaveAsync(request.ChaveIdempotencia);
        if (idempotencia != null)
        {
            return JsonSerializer.Deserialize<MovimentacaoResponse>(idempotencia.Resultado);
        }

        // Validações de negócio
        var conta = await _contaCorrenteRepository.ObterPorIdAsync(request.IdContaCorrente);
        if (conta == null)
        {
            throw DomainException.ContaInvalida();
        }
        if (!conta.EstaAtiva())
        {
            throw DomainException.ContaInativa();
        }
        if (request.Valor <= 0)
        {
            throw DomainException.ValorInvalido();
        }
        if (request.TipoMovimentacao != "C" && request.TipoMovimentacao != "D")
        {
            throw DomainException.TipoInvalido();
        }

        // Processar a transação
        var movimentacao = new Movimentacao(request.IdContaCorrente, request.TipoMovimentacao, request.Valor);
        var idMovimentacao = await _movimentoRepository.InserirAsync(movimentacao);

        // Persistir idempotência
        var response = new MovimentacaoResponse { IdMovimentacao = idMovimentacao };
        await _idempotenciaRepository.InserirAsync(new Idempotencia(
            request.ChaveIdempotencia,
            JsonSerializer.Serialize(request),
            JsonSerializer.Serialize(response)
        ));

        return response;
    }
}
