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
    protected (string? whereFilter, Dictionary<string, object> values)? GrokQuery(string query, bool hasQuery = true)
    {
        var queryObject = Base62JsonConverter.DeserializeAnonymous(query, null as Dictionary<string, string>);
        if (queryObject == null)
            return null;

        var stringBuilder = new StringBuilder();
        var values = new Dictionary<string, object>();

        if (hasQuery)
            stringBuilder.Append(" AND ");

        foreach (var key in queryObject.Keys)
        {
            var snakeName = ToSnakeCase(key);
            if (!Columns.ContainsKey(snakeName) || !queryObject.TryGetValue(key, out var value) || value is null)
                continue;

            object parsedValue;
            if (int.TryParse(value, out int intValue))
            {
                parsedValue = intValue;
                stringBuilder.Append($"{snakeName}=@{snakeName} AND ");
            }
            else if (decimal.TryParse(value, out decimal decimalValue))
            {
                parsedValue = decimalValue;
                stringBuilder.Append($"{snakeName}=@{snakeName} AND ");
            }
            else if (float.TryParse(value, out float floatValue))
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

    public Task<bool> ExistsAsync(int id) => ConnectionProvider.Connection.ExecuteScalarAsync<bool>(
        $"SELECT EXISTS(SELECT 1 FROM {TableName} WHERE id = @id)", new { id }
    );
    
    public virtual IAsyncEnumerable<T> GetSome(int count) => GetNext(0, count, null);
    public virtual IAsyncEnumerable<T> GetSome(int count, string query) => GetNext(0, count, query);
    public virtual IAsyncEnumerable<T> GetNext(int id, int count) => GetNext(id, count, null);
    public virtual async IAsyncEnumerable<T> GetNext(int id, int count, string query)
    {
        var queryObj = GrokQuery(query, !string.IsNullOrEmpty(query));
        var whereFilter = queryObj?.whereFilter ?? "";
        var values = queryObj?.values ?? new Dictionary<string, object>();
        values["id"] = id;
        var results = await ConnectionProvider.Connection.QueryAsync<T>(
            $"SELECT * FROM {TableName} WHERE id > @id {whereFilter} ORDER BY id ASC LIMIT @count",
            values
        );

        foreach (var item in results)
            yield return item;
    }
    
    public virtual IAsyncEnumerable<T> GetPrevious(int id, int count)=>GetPrevious(id, count, null);
    public virtual async IAsyncEnumerable<T> GetPrevious(int id, int count, string query)
    {
        var queryObj = GrokQuery(query, !string.IsNullOrEmpty(query));
        var whereFilter = queryObj?.whereFilter ?? "";
        var values = queryObj?.values ?? new Dictionary<string, object>();
        values.Add("lastId", id);
        values.Add("count", count);
        var results = await ConnectionProvider.Connection.QueryAsync<T>(
            $"SELECT * FROM {TableName} WHERE id < @id {whereFilter} ORDER BY id DESC LIMIT @count",
            values
        );

        foreach (var item in results.Reverse()) // Reverse to maintain ascending order
            yield return item;
    }

    public virtual Task<T?> FindAsync(int id) => ConnectionProvider.Connection.QueryFirstOrDefaultAsync<T>(
        $"SELECT * FROM {TableName} WHERE id = @id", new { id }
    );
    public virtual Task<int> UpdateAsync(T entity) =>
        ConnectionProvider.Connection.ExecuteAsync($@"
        UPDATE {TableName}
        SET {_updateClause}
        WHERE id = @Id
        AND (@UpdatedTs IS NULL OR updated_ts = @UpdatedTs)", entity);


    public virtual Task<int> DeleteAsync(int id) => ConnectionProvider.Connection.ExecuteAsync(
        $"DELETE FROM {TableName} WHERE id = @id", new { id }
    );

    public Task<int> CreateAsync(T entity) =>
        ConnectionProvider.Connection.ExecuteScalarAsync<int>(
            @$"INSERT INTO {TableName} ({_insertLeftClause}) 
                   VALUES ({_insertRightClause}) RETURNING id", entity
        );
}