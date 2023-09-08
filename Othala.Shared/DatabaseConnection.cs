using System.Data;
using MySql.Data.MySqlClient;

namespace Othala.Shared;

public interface IDatabaseConnection
{
    Task<IDbConnection> GetConnection();
}

public class DatabaseConnection : IDatabaseConnection
{
    private readonly string _connectionString;

    public DatabaseConnection(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IDbConnection> GetConnection()
    {
        var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        return connection;
    }
}