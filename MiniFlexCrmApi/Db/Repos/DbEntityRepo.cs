using System.Text.RegularExpressions;
using Dapper;
using MiniFlexCrmApi.Db.Models;

namespace MiniFlexCrmApi.Db.Repos;

public class DbEntityRepo<T> : IRepo<T> where T : DbEntity
{
    /// <summary>
    /// Converts PascalCase or camelCase to snake_case for PostgreSQL compatibility.
    /// </summary>
    private static string ToSnakeCase(string input)
    {
        return Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
    }
    
    protected readonly IConnectionProvider ConnectionProvider;
    protected readonly string TableName;
    private readonly string _insertLeftClause;
    private readonly string _insertRightClause;
    private readonly string _updateClause;

    public DbEntityRepo(IConnectionProvider connectionProvider)
    {
        ConnectionProvider = connectionProvider;
        TableName = typeof(T).Name.Replace("DbModel", "").ToLower(); // Ensure table name is lowercase
        var propertyNames = typeof(T).GetProperties()
            .Where(p => p.Name.ToLower() != "id")
            .OrderBy(p => p.Name)
            .Select(p => p.Name)
            .ToList();
        _insertLeftClause = string.Join(", ", propertyNames.Select(ToSnakeCase));
        _insertRightClause = string.Join(", ", propertyNames.Select(n=>"@"+n));
        _updateClause = String.Join(", ", propertyNames
            .Select(n => $"{ToSnakeCase(n)}=@{n}"));
    }

    public Task<bool> ExistsAsync(int id) => ConnectionProvider.Connection.ExecuteScalarAsync<bool>(
        $"SELECT EXISTS(SELECT 1 FROM {TableName} WHERE id = @id)", new { id }
    );
    
    public IAsyncEnumerable<T> GetSome(int count) => GetNext(0, count);

    public async IAsyncEnumerable<T> GetNext(int startId, int count)
    {
        var results = await ConnectionProvider.Connection.QueryAsync<T>(
            $"SELECT * FROM {TableName} WHERE id >= @startId ORDER BY id ASC LIMIT @count",
            new { startId, count }
        );

        foreach (var item in results)
            yield return item;
    }

    public async IAsyncEnumerable<T> GetPrevious(int startId, int count)
    {
        var results = await ConnectionProvider.Connection.QueryAsync<T>(
            $"SELECT * FROM {TableName} WHERE id <= @startId ORDER BY id DESC LIMIT @count",
            new { startId, count }
        );

        foreach (var item in results.Reverse()) // Reverse to maintain ascending order
            yield return item;
    }

    public Task<T?> FindAsync(int id) => ConnectionProvider.Connection.QueryFirstOrDefaultAsync<T>(
        $"SELECT * FROM {TableName} WHERE id = @id", new { id }
    );

    public Task<int> UpdateAsync(T entity) => 
        ConnectionProvider.Connection.ExecuteAsync(
            $"UPDATE {TableName} SET {_updateClause} WHERE id = @Id", entity
        );

    public Task<int> DeleteAsync(int id) => ConnectionProvider.Connection.ExecuteAsync(
        $"DELETE FROM {TableName} WHERE id = @id", new { id }
    );

    public Task<int> CreateAsync(T entity) =>
        ConnectionProvider.Connection.ExecuteScalarAsync<int>(
            @$"INSERT INTO {TableName} ({_insertLeftClause}) 
                   VALUES ({_insertRightClause}) RETURNING id", entity
        );
}