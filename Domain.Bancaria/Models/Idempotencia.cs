namespace API.Bancaria.Models;

public class Idempotencia
{
    public string ChaveIdempotencia { get; private set; }

    public string Requisicao { get; private set; }

    public string Resultado { get; private set; }

    public Idempotencia(string chaveIdempotencia, string requisicao, string resultado)
    {
        ChaveIdempotencia = chaveIdempotencia;
        Requisicao = requisicao;
        Resultado = resultado;
    }
}
