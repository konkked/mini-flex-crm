using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using MiniFlexCrmApi.Db.Models;

namespace MiniFlexCrmApi.Db.Repos;

public class TenantBoundDbEntityRepo<T>(IConnectionProvider connectionProvider)
    : DbEntityRepo<T>(connectionProvider), IRepo<T>
    where T : TenantBoundDbEntity
{
    public override IAsyncEnumerable<T> GetSomeAsync(int limit) => GetSomeAsync(limit, 0);

    public override async IAsyncEnumerable<T> GetSomeAsync(int limit, int offset)
    {
        await using var connection = ConnectionProvider.GetConnection();
        await connection.OpenAsync().ConfigureAwait(false);
        var results = await connection.QueryAsync<T>(
            @$"SELECT t1.*, t2.name as tenant_name
                   FROM {TableName} t1
                   JOIN tenant t2 
                       on t1.tenant_id = t2.id   
                   ORDER BY t1.id ASC 
                   LIMIT @count
                   OFFSET @offset",
            new { offset, count = limit }
        ).ConfigureAwait(false);

        foreach (var item in results)
            yield return item;
    }

    
    public override IAsyncEnumerable<T> GetSomeAsync(int limit, string query) => GetSomeAsync(limit, 0, null);

    public override async IAsyncEnumerable<T> GetSomeAsync(int limit, int offset, string query)
    {
        var queryObj = GrokQuery(query);
        
        var whereFilter = queryObj?.whereFilter ?? "";
        var values = queryObj?.values ?? new Dictionary<string, object>();
        values["offset"] = offset;
        values["count"] = limit;
        await using var connection = ConnectionProvider.GetConnection();
        await connection.OpenAsync().ConfigureAwait(false);
        var results = await connection.QueryAsync<T>(
            @$"SELECT t1.*, t2.name as tenant_name
                   FROM {TableName} t1
                   JOIN tenant t2 
                       on t1.tenant_id = t2.id 
                    {(!string.IsNullOrEmpty(whereFilter) ? $"WHERE {whereFilter}" : string.Empty)}
                   ORDER BY t1.id ASC 
                   LIMIT @count
                   OFFSET @offset",
            values
        );

        foreach (var item in results)
            yield return item;
    }

    public override async Task<T?> FindAsync(int id)
    {
        await using var connection = ConnectionProvider.GetConnection();
        await connection.OpenAsync().ConfigureAwait(false);
        return await connection.QueryFirstOrDefaultAsync<T>(
            @$"SELECT t1.*, t2.name as tenant_name 
                FROM {TableName} t1 
                    JOIN tenant t2 on t1.tenant_id = t2.id
                WHERE id = @id",
            new { id }
        ).ConfigureAwait(false);
    }
}