namespace API.Bancaria.Models;

public class Movimentacao
{
    public string IdMovimentacao { get; private set; }
    public string IdContaCorrente { get; private set; }
    public string DataMovimentacao { get; private set; }
    public string TipoMovimentacao { get; private set; }
    public decimal Valor { get; private set; }

    public Movimentacao()
    {
    }

    public Movimentacao(string idContaCorrente, string tipoMovimentacao, decimal valor)
    {
        IdMovimentacao = Guid.NewGuid().ToString();
        IdContaCorrente = idContaCorrente;
        DataMovimentacao = DateTime.Now.ToString("dd/MM/yyyy");
        TipoMovimentacao = tipoMovimentacao;
        Valor = valor;
    }
}
