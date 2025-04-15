using API.Bancaria.Models;
using Dapper;
using Domain.Bancaria.Repositories;
using Microsoft.Data.Sqlite;
using System.Data;

namespace Infrastructure.Repositories;
public class IdempotenciaRepository : IIdempotenciaRepository
{
    private readonly string _connectionString;

    public IdempotenciaRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<Idempotencia> ObterPorChaveAsync(string chaveIdempotencia)
    {
        const string sql = @"
                SELECT chave_idempotencia as ChaveIdempotencia, 
                       requisicao as Requisicao, 
                       resultado as Resultado
                FROM idempotencia 
                WHERE chave_idempotencia = @ChaveIdempotencia";

        using (IDbConnection connection = new SqliteConnection(_connectionString))
        {
            return await connection.QueryFirstOrDefaultAsync<Idempotencia>(
                sql,
                new { ChaveIdempotencia = chaveIdempotencia }
            );
        }
    }

    public async Task InserirAsync(Idempotencia idempotencia)
    {
        const string sql = @"
                INSERT INTO idempotencia (chave_idempotencia, requisicao, resultado)
                VALUES (@ChaveIdempotencia, @Requisicao, @Resultado)";

        using (IDbConnection connection = new SqliteConnection(_connectionString))
        {
            await connection.ExecuteAsync(
                sql,
                new
                {
                    ChaveIdempotencia = idempotencia.ChaveIdempotencia,
                    Requisicao = idempotencia.Requisicao,
                    Resultado = idempotencia.Resultado
                }
            );
        }
    }
}
