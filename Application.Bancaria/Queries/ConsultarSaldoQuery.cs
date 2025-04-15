using API.Bancaria.Response;
using MediatR;

namespace API.Bancaria.Query;

public class ConsultarSaldoQuery : IRequest<SaldoContaCorrenteResponse>
{
    public string IdContaCorrente { get; set; }
}
