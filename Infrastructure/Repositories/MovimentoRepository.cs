using API.Bancaria.Models;
using API.Bancaria.Repository;
using Dapper;
using Microsoft.Data.Sqlite;
using System.Data;

namespace Infrastructure.Repositories;
public class MovimentoRepository : IMovimentoRepository
{
    private readonly string _connectionString;

    public MovimentoRepository(string connectionString)
    {
        _connectionString = connectionString;
    }


    public async Task<string> InserirAsync(Movimentacao movimentacao)
    {
        const string sql = @"
                INSERT INTO movimentacao (idmovimentacao, idcontacorrente, datamovimentacao, tipomovimentacao, valor)
                VALUES (@IdMovimentacao, @IdContaCorrente, @DataMovimentacao, @TipoMovimentacao, @Valor)";

        using (IDbConnection connection = new SqliteConnection(_connectionString))
        {
            await connection.ExecuteAsync(
             sql,
                 new
                 {
                     IdMovimentacao = movimentacao.IdMovimentacao,
                     IdContaCorrente = movimentacao.IdContaCorrente,
                     DataMovimentacao = movimentacao.DataMovimentacao,
                     TipoMovimentacao = movimentacao.TipoMovimentacao,
                     Valor = movimentacao.Valor
                 }
 );

            return movimentacao.IdMovimentacao;
        }
    }

    public async Task<IEnumerable<Movimentacao>> ObterPorContaCorrenteAsync(string idContaCorrente)
    {
        const string sql = @"
                SELECT idmovimentacao as IdMovimento, 
                       idcontacorrente as IdContaCorrente, 
                       datamovimentacao as DataMovimentacao, 
                       tipomovimentacao as TipoMovimentacao, 
                       valor as Valor
                FROM movimentacao 
                WHERE idcontacorrente = @IdContaCorrente";

        using (IDbConnection connection = new SqliteConnection(_connectionString))
        {
          

            var movimentos = await connection.QueryAsync<Movimentacao>(
                sql,
                new { IdContaCorrente = idContaCorrente }
            );

            return movimentos.ToList();
        }
    }
}
