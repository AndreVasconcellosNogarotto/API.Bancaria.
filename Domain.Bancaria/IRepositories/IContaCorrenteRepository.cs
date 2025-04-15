using API.Bancaria.Models;

namespace API.Bancaria.Repository;

public interface IContaCorrenteRepository
{
    Task<ContaCorrente> ObterPorIdAsync(string idContaCorrente);
}
