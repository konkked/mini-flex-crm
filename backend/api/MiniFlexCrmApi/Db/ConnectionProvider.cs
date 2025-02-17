using System;
using System.Data.Common;
using Npgsql;

namespace MiniFlexCrmApi.Db;

public class ConnectionProvider(string connectionString) : IConnectionProvider
{
    private readonly Lazy<DbConnection> _connection 
        = new(() => string.IsNullOrEmpty(connectionString) 
            ? throw new ArgumentException(nameof(connectionString)) 
            : new NpgsqlConnection(connectionString));

    public DbConnection Connection => _connection.Value;
}