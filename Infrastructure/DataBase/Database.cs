using Microsoft.Data.Sqlite;
using SQLitePCL;

namespace Infrastructure.DataBase;
public static class Database
{
    public static void Initialize(string connectionString)
    {
        try
        {
            Console.WriteLine("Inicializando banco de dados SQLite...");

            Batteries.Init();

            var builder = new SqliteConnectionStringBuilder(connectionString);
            string dbFile = builder.DataSource;

            Console.WriteLine($"Arquivo do banco de dados: {dbFile}");

            string? directory = Path.GetDirectoryName(dbFile);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                Console.WriteLine($"Diretório criado: {directory}");
            }

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                Console.WriteLine("Conexão com o banco de dados aberta com sucesso");

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                            CREATE TABLE IF NOT EXISTS contacorrente (
                                idcontacorrente TEXT PRIMARY KEY,
                                numero INTEGER NOT NULL UNIQUE,
                                nome TEXT NOT NULL,
                                ativo INTEGER NOT NULL DEFAULT 0,
                                CHECK (ativo IN (0, 1))
                            );
                            
                            CREATE TABLE IF NOT EXISTS movimentacao (
                                idmovimentacao TEXT PRIMARY KEY,
                                idcontacorrente TEXT NOT NULL,
                                datamovimentacao TEXT NOT NULL,
                                tipomovimentacao TEXT NOT NULL,
                                valor REAL NOT NULL,
                                CHECK (tipomovimentacao IN ('C', 'D')),
                                FOREIGN KEY(idcontacorrente) REFERENCES contacorrente(idcontacorrente)
                            );
                            
                            CREATE TABLE IF NOT EXISTS idempotencia (
                                chave_idempotencia TEXT PRIMARY KEY,
                                requisicao TEXT,
                                resultado TEXT
                            );
                        ";

                    command.ExecuteNonQuery();
                    Console.WriteLine("Tabelas criadas com sucesso");
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT COUNT(*) FROM contacorrente";
                    long count = (long)command.ExecuteScalar();

                    if (count == 0)
                    {
                        using (var insertCommand = connection.CreateCommand())
                        {
                            insertCommand.CommandText = @"
                                    INSERT INTO contacorrente(idcontacorrente, numero, nome, ativo) 
                                    VALUES('B6BAFC09-6967-ED11-A567-055DFA4A16C9', 123, 'André Vasconcellos', 1);
                                    
                                    INSERT INTO contacorrente(idcontacorrente, numero, nome, ativo) 
                                    VALUES('FA99D033-7067-ED11-96C6-7C5DFA4A16C9', 456, 'Adrieli Dias', 1);
                                    
                                    INSERT INTO contacorrente(idcontacorrente, numero, nome, ativo) 
                                    VALUES('382D323D-7067-ED11-8866-7D5DFA4A16C9', 789, 'João da Silva', 1);
                                    
                                    INSERT INTO contacorrente(idcontacorrente, numero, nome, ativo) 
                                    VALUES('F475F943-7067-ED11-A06B-7E5DFA4A16C9', 741, 'Ana Silva', 0);
                                    
                                    INSERT INTO contacorrente(idcontacorrente, numero, nome, ativo) 
                                    VALUES('BCDACA4A-7067-ED11-AF81-825DFA4A16C9', 852, 'Maria Maia', 0);
                                    
                                    INSERT INTO contacorrente(idcontacorrente, numero, nome, ativo) 
                                    VALUES('D2E02051-7067-ED11-94C0-835DFA4A16C9', 963, 'Francisco Oliveira', 0);
                                ";

                            insertCommand.ExecuteNonQuery();
                            Console.WriteLine("Dados de exemplo inseridos com sucesso");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Tabela contacorrente já contém {count} registros");
                    }
                }
            }

            Console.WriteLine("Inicialização do banco de dados concluída com sucesso");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERRO na inicialização do banco de dados: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");

            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }

            throw; 
        }
    }
}


