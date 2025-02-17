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
    public override IAsyncEnumerable<T> GetSome(int count) => GetNext(0, count);

    public override async IAsyncEnumerable<T> GetNext(int id, int count)
    {
        var results = await ConnectionProvider.Connection.QueryAsync<T>(
            @$"SELECT t1.*, t2.name as tenant_name
                   FROM {TableName} t1
                   JOIN tenant t2 
                       on t1.tenant_id = t2.id  
                   WHERE t1.id > @id 
                   ORDER BY t1.id ASC 
                   LIMIT @count",
            new { id, count }
        );

        foreach (var item in results)
            yield return item;
    }

    public override async IAsyncEnumerable<T> GetPrevious(int id, int count)
    {
        var results = await ConnectionProvider.Connection.QueryAsync<T>(
            @$"SELECT t1.*, t2.name as tenant_name
                   FROM {TableName} t1
                   JOIN tenant t2 
                       on t1.tenant_id = t2.id 
                   WHERE t1.id <= @id 
                   ORDER BY t1.id DESC 
                   LIMIT @count",
            new { id, count }
        );

        foreach (var item in results.Reverse()) // Reverse to maintain ascending order
            yield return item;
    }
    
    public override IAsyncEnumerable<T> GetSome(int count, string query) => GetNext(0, count, null);

    public override async IAsyncEnumerable<T> GetNext(int id, int count, string query)
    {
        var queryObj = GrokQuery(query, !string.IsNullOrEmpty(query));
        var whereFilter = queryObj?.whereFilter ?? "";
        var values = queryObj?.values ?? new Dictionary<string, object>();
        values["id"] = id;
        values["count"] = count;
        var results = await ConnectionProvider.Connection.QueryAsync<T>(
            @$"SELECT t1.*, t2.name as tenant_name
                   FROM {TableName} t1
                   JOIN tenant t2 
                       on t1.tenant_id = t2.id  
                   WHERE t1.id > @id {whereFilter}
                   ORDER BY t1.id ASC 
                   LIMIT @count",
            values
        );

        foreach (var item in results)
            yield return item;
    }

    public override async IAsyncEnumerable<T> GetPrevious(int id, int count, string query)
    {
        var queryObj = GrokQuery(query, !string.IsNullOrEmpty(query));
        var whereFilter = queryObj?.whereFilter ?? "";
        var values = queryObj?.values ?? new Dictionary<string, object>();
        values["id"] = id;
        values["count"] = count;
        var results = await ConnectionProvider.Connection.QueryAsync<T>(
            @$"SELECT t1.*, t2.name as tenant_name
                   FROM {TableName} t1
                   JOIN tenant t2 
                       on t1.tenant_id = t2.id 
                   WHERE t1.id <= @id {whereFilter}
                   ORDER BY t1.id DESC 
                   LIMIT @count",
            values
        );

        foreach (var item in results.Reverse()) // Reverse to maintain ascending order
            yield return item;
    }

    public override Task<T?> FindAsync(int id) => ConnectionProvider.Connection.QueryFirstOrDefaultAsync<T>(
        @$"SELECT t1.*, t2.name as tenant_name 
                FROM {TableName} t1 
                    JOIN tenant t2 on t1.tenant_id = t2.id
                WHERE id = @id", 
        new { id }
    );
}