using API.Bancaria.Models;

namespace Domain.Bancaria.Repositories;
public interface IIdempotenciaRepository
{
    Task<Idempotencia> ObterPorChaveAsync(string chaveIdempotencia);

    Task InserirAsync(Idempotencia idempotencia);
}
