using API.Bancaria.Models;
using API.Bancaria.Repository;
using Microsoft.Data.Sqlite;
using System.Data;
using Dapper;

namespace API.Bancaria.Infrastrutuce;

public class ContaCorrenteRepository : IContaCorrenteRepository
{
    private readonly string _connectionString;

    public ContaCorrenteRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<ContaCorrente> ObterPorIdAsync(string idContaCorrente)
    {
        const string sql = @"
            SELECT idcontacorrente as IdContaCorrente, 
                   numero as Numero, 
                   nome as Nome, 
                   ativo as Ativo
            FROM contacorrente 
            WHERE idcontacorrente = @IdContaCorrente";

        using (IDbConnection connection = new SqliteConnection(_connectionString))
        {
            return await connection.QueryFirstOrDefaultAsync<ContaCorrente>(
                sql,
                new { IdContaCorrente = idContaCorrente }
            );
        }
    }
}
