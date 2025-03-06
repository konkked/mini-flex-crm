using Dapper;
using Microsoft.Extensions.Logging.Abstractions;
using MiniFlexCrmApi.Db.Models;

namespace MiniFlexCrmApi.Db.Repos;

public interface ITenantBoundDbEntityRepo<T> : IRepo<T> where T: TenantBoundDbEntity
{
    public Task<bool> DeleteAsync(int tenantId, int id);

    public Task<T?> FindInTenantById(int id, int tenantId);
}

public class TenantBoundDbEntityRepo<T>(IConnectionProvider connectionProvider)
    : DbEntityRepo<T>(connectionProvider), ITenantBoundDbEntityRepo<T>
    where T : TenantBoundDbEntity
{
    
    public override IAsyncEnumerable<T> GetSomeAsync(int limit) => GetSomeAsync(limit, 0);

    public override IAsyncEnumerable<T> GetSomeAsync(int limit, int offset)
        => GetSomeAsync(limit, offset, null, null);
    
    public override IAsyncEnumerable<T> GetSomeAsync(int limit, string? query) 
        => GetSomeAsync(limit, 0, query);

    public override IAsyncEnumerable<T> GetSomeAsync(int limit, int offset, string? query)
        => GetSomeAsync(limit, offset, query, null);

    public override async IAsyncEnumerable<T> GetSomeAsync(int limit, int offset, string? query,
        IDictionary<string, object>? parameters)
    {
        if (parameters?.TryGetValue("tenant_id", out var tenantId) == true && tenantId is 0)
            parameters.Remove("tenant_id");
        
        var queryObj = !string.IsNullOrEmpty(query) ? GrokQuery(query, parameters) : null;
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

    public async Task<bool> DeleteAsync(int tenantId, int id)
    {
        if(tenantId == 0) return await base.DeleteAsync(id) == 1;
        await using var connection = ConnectionProvider.GetConnection();
        await connection.OpenAsync().ConfigureAwait(false);
        return (await connection.ExecuteAsync($"DELETE FROM {TableName} WHERE id = @id and tenant_id = @tenantId",
            new { id, tenantId }).ConfigureAwait(false)) == 1;
    }

    public async Task<T?> FindInTenantById(int id, int tenantId)
    {
        if(tenantId == 0) return await FindAsync(id).ConfigureAwait(false);
        await using var connection = ConnectionProvider.GetConnection();
        await connection.OpenAsync().ConfigureAwait(false);
        return await connection.QueryFirstOrDefaultAsync<T>(
            $"SELECT * FROM {TableName} WHERE tenant_id = @tenant_id and id = @id", new
                { id, tenantId }).ConfigureAwait(false);
    }
}