namespace Domain.Bancaria.Exceptions;

public class DomainException : Exception
{
    public string Tipo { get; }

    public DomainException(string message, string tipo) : base(message)
    {
        Tipo = tipo;
    }

    public static DomainException ContaInvalida(string mensagem = "Conta corrente não encontrada")
    {
        return new DomainException(mensagem, "INVALID_ACCOUNT");
    }

    public static DomainException ContaInativa(string mensagem = "Conta corrente inativa")
    {
        return new DomainException(mensagem, "INACTIVE_ACCOUNT");
    }

    public static DomainException ValorInvalido(string mensagem = "Valor da movimentação deve ser positivo")
    {
        return new DomainException(mensagem, "INVALID_VALUE");
    }

    public static DomainException TipoInvalido(string mensagem = "Tipo de movimento inválido, deve ser C (Crédito) ou D (Débito)")
    {
        return new DomainException(mensagem, "INVALID_TYPE");
    }
}