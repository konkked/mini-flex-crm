using System.Data.Common;
using Npgsql;

namespace MiniFlexCrmApi.Db;

public class ConnectionProvider : IConnectionProvider
{
    private readonly Lazy<DbConnection> _connection;
    public ConnectionProvider(string connectionString)
        => _connection = new Lazy<DbConnection>(() => new NpgsqlConnection(connectionString));
    
    public DbConnection Connection => _connection.Value;
}