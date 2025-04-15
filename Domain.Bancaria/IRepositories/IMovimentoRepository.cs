using API.Bancaria.Models;

namespace API.Bancaria.Repository;

public interface IMovimentoRepository
{
    Task<string> InserirAsync(Movimentacao movimentacao);

    Task<IEnumerable<Movimentacao>> ObterPorContaCorrenteAsync(string idContaCorrente);
}
