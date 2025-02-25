using System;
using System.Data.Common;
using Npgsql;

namespace MiniFlexCrmApi.Db;

public class ConnectionProvider(string connectionString) : IConnectionProvider
{

    public DbConnection GetConnection() => new NpgsqlConnection(connectionString);
}