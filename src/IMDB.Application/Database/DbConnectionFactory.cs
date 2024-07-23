using System.Data;
using Npgsql;

namespace IMDB.Application.Database;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync(CancellationToken token = default);
}

public class NpgsqlConnectionFactory(string _connectionString) : IDbConnectionFactory
{
    public async Task<IDbConnection> CreateConnectionAsync(CancellationToken token)
    {
        NpgsqlConnection connection = new(_connectionString);

        await connection.OpenAsync(token);

        return connection;
    }
}