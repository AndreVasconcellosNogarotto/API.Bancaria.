namespace API.Bancaria.Response;

public class SaldoContaCorrenteResponse
{
    public int NumeroConta { get; set; }

    public string NomeTitular { get; set; }

    public DateTime DataConsulta { get; set; }

    public decimal Saldo { get; set; }
}
