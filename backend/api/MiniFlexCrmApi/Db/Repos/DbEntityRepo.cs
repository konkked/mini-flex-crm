using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dapper;
using MiniFlexCrmApi.Api.Services;
using MiniFlexCrmApi.Db.Models;

namespace MiniFlexCrmApi.Db.Repos;

public class DbEntityRepo<T> : IRepo<T> where T : DbEntity
{
    protected (string? whereFilter, Dictionary<string, object> values)? GrokQuery(string query)
    {
        var queryObject = Base62JsonConverter.DeserializeAnonymous(query, null as Dictionary<string, string>);
        if (queryObject == null)
            return null;

        var stringBuilder = new StringBuilder();
        var values = new Dictionary<string, object>();

        foreach (var key in queryObject.Keys)
        {
            var snakeName = ToSnakeCase(key);
            if (!Columns.ContainsKey(snakeName) || !queryObject.TryGetValue(key, out var value) || value is null)
                continue;

            object parsedValue;
            if (int.TryParse(value, out var intValue))
            {
                parsedValue = intValue;
                stringBuilder.Append($"{snakeName}=@{snakeName} AND ");
            }
            else if (decimal.TryParse(value, out var decimalValue))
            {
                parsedValue = decimalValue;
                stringBuilder.Append($"{snakeName}=@{snakeName} AND ");
            }
            else if (float.TryParse(value, out var floatValue))
            {
                parsedValue = floatValue;
                stringBuilder.Append($"{snakeName}=@{snakeName} AND ");
            }
            else
            {
                parsedValue = value.ToString();
                stringBuilder.Append($"{snakeName} LIKE '%' || @{snakeName} || '%' AND ");
            }

            // SQL injection check
            if (snakeName.Contains(";") || snakeName.Contains("--") 
                                        || snakeName.Contains("/*") || snakeName.Contains("*/"))
            {
                throw new ArgumentException("Invalid query parameter");
            }

            values.Add(snakeName, parsedValue);
        }

        // Remove the trailing " AND "
        if (stringBuilder.Length > 5)
        {
            stringBuilder.Length -= 5;
        }

        return (stringBuilder.ToString(), values);
    }

    /// <summary>
    /// Converts PascalCase or camelCase to snake_case for PostgreSQL compatibility.
    /// </summary>
    protected static string ToSnakeCase(string input)
    {
        return Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
    }
    
    protected readonly IConnectionProvider ConnectionProvider;
    protected readonly Dictionary<string, PropertyInfo> Columns;
    protected readonly string TableName;
    private readonly string _insertLeftClause;
    private readonly string _insertRightClause;
    private readonly string _updateClause;

    public DbEntityRepo(IConnectionProvider connectionProvider)
    {
        ConnectionProvider = connectionProvider;
        TableName = typeof(T).Name.Replace("DbModel", "").ToLower(); // Ensure table name is lowercase
        var properties = typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
            .ToList();
        Columns = properties.ToDictionary(x => ToSnakeCase(x.Name), x => x, StringComparer.OrdinalIgnoreCase);
        var propertyNames = properties 
            .Where(p => !p.Name.Equals("id", StringComparison.OrdinalIgnoreCase)) 
            .Where(p => p.GetCustomAttribute<IgnoreForUpdateAttribute>() == null) 
            .OrderBy(p => p.Name)
            .Select(p => p.Name)
            .ToList();
        var columnNames = propertyNames.Select(ToSnakeCase).ToArray();
        _insertLeftClause = string.Join(", ", columnNames);
        _insertRightClause = string.Join(", ", propertyNames.Select(n=>"@"+n));
        _updateClause = String.Join(", ", propertyNames
            .Select(n => $"{ToSnakeCase(n)}=@{n}"));
    }

    public async Task<bool> ExistsAsync(int id)
    {
        await using var connection = ConnectionProvider.GetConnection();
        await connection.OpenAsync().ConfigureAwait(false);
        return await connection.ExecuteScalarAsync<bool>(
        $"SELECT EXISTS(SELECT 1 FROM {TableName} WHERE id = @id)", new { id }
        ).ConfigureAwait(false);
    }

    public virtual IAsyncEnumerable<T> GetSomeAsync() => GetSomeAsync(ServiceContants.PageSize, 0);
    public virtual IAsyncEnumerable<T> GetSomeAsync(int limit) => GetSomeAsync(limit, 0, null);
    public virtual IAsyncEnumerable<T> GetSomeAsync(int limit, string query) => GetSomeAsync(limit, 0, query);
    public virtual IAsyncEnumerable<T> GetSomeAsync(int limit, int offset) => GetSomeAsync(limit, offset, null);
    public virtual async IAsyncEnumerable<T> GetSomeAsync(int limit, int offset, string query)
    {
        var queryObj = GrokQuery(query);
        var whereFilter = queryObj?.whereFilter ?? "";
        var values = queryObj?.values ?? new Dictionary<string, object>();
        values["offset"] = offset;
        values["count"] = limit;
        await using var connection = ConnectionProvider.GetConnection();
        await connection.OpenAsync().ConfigureAwait(false);
        var results = await connection.QueryAsync<T>(
            $@"SELECT * 
                   FROM {TableName} 
                    {(!string.IsNullOrEmpty(whereFilter) ? $"WHERE {whereFilter}" : string.Empty)} 
                   ORDER BY id ASC 
                   LIMIT @count 
                   OFFSET @offset ",
            values
        ).ConfigureAwait(false);

        foreach (var item in results)
            yield return item;
    }

    public virtual async Task<T?> FindAsync(int id)
    {
        await using var connection = ConnectionProvider.GetConnection();
        await connection.OpenAsync().ConfigureAwait(false);
        return await connection.QueryFirstOrDefaultAsync<T>(
            $"SELECT * FROM {TableName} WHERE id = @id", new { id }
        ).ConfigureAwait(false);
    }

    public virtual async Task<int> UpdateAsync(T entity)
    {
        await using var connection = ConnectionProvider.GetConnection();
        await connection.OpenAsync().ConfigureAwait(false);
        return await connection.ExecuteAsync($@"
        UPDATE {TableName}
        SET {_updateClause}
        WHERE id = @Id
        AND (@UpdatedTs IS NULL OR updated_ts = @UpdatedTs)", entity);
    }
    
    public virtual async Task<int> DeleteAsync(int id)
    {
        await using var connection = ConnectionProvider.GetConnection();
        await connection.OpenAsync().ConfigureAwait(false);
        return await connection.ExecuteAsync(
            $"DELETE FROM {TableName} WHERE id = @id", new { id }
        ).ConfigureAwait(false);
    }

    public async Task<int> CreateAsync(T entity)
    {
        await using var connection = ConnectionProvider.GetConnection();
        await connection.OpenAsync().ConfigureAwait(false);
        return await connection.ExecuteScalarAsync<int>(
            @$"INSERT INTO {TableName} ({_insertLeftClause}) 
                   VALUES ({_insertRightClause}) RETURNING id", entity
        ).ConfigureAwait(false);
    }
}