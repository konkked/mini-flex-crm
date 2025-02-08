using System.Text.Json;
using Dapper;
using MiniFlexCrmApi.Db.Models;

namespace MiniFlexCrmApi.Db.Repos;

public class RelationRepo : DbEntityRepo<RelationDbModel>, IRelationRepo
{
    public RelationRepo(IConnectionProvider connectionProvider) : base(connectionProvider)
    {
    }

    /// <summary>
    /// Retrieves aggregated relationships for a given customer.
    /// Calls PostgreSQL function get_customer_entities(customer_id) and returns parsed data.
    /// </summary>
    public async Task<Dictionary<string,dynamic[]>> GetCustomerRelationshipsAsync(int customerId)
    {
        var result = await ConnectionProvider.Connection
            .QueryFirstOrDefaultAsync<string>(
                "SELECT get_customer_entities(@customerId) AS relationships_json", 
                new { customerId })
            .ConfigureAwait(false);

        if (string.IsNullOrEmpty(result))
            return null;

        // Convert JSON result into a Dictionary<string, dynamic[]>
        
        return JsonSerializer.Deserialize<Dictionary<string, dynamic[]>>(result);
    }
    
    public override IAsyncEnumerable<RelationDbModel> GetSome(int count) => GetNext(0, count);

    public override async IAsyncEnumerable<RelationDbModel> GetNext(int lastId, int count)
    {
        var results = await ConnectionProvider.Connection.QueryAsync<RelationDbModel>(
            @$"SELECT t1.*, t2.name as customer_name
                   FROM {TableName} t1
                   JOIN customer t2 
                       on t1.customer_id = t2.id  
                   WHERE t1.id >= @startId 
                   ORDER BY t1.id ASC 
                   LIMIT @count",
            new { startId = lastId, count }
        );

        foreach (var item in results)
            yield return item;
    }

    public override async IAsyncEnumerable<RelationDbModel> GetPrevious(int lastId, int count)
    {
        var results = await ConnectionProvider.Connection.QueryAsync<RelationDbModel>(
            @$"SELECT t1.*, t2.name as customer_name
                   FROM {TableName} t1
                   JOIN customer t2 
                       on t1.customer_id = t2.id  
                   WHERE t1.id <= @startId 
                   ORDER BY t1.id DESC 
                   LIMIT @count",
            new { startId = lastId, count }
        );

        foreach (var item in results.Reverse()) // Reverse to maintain ascending order
            yield return item;
    }

    public override Task<RelationDbModel?> FindAsync(int id) => 
        ConnectionProvider.Connection.QueryFirstOrDefaultAsync<RelationDbModel>(
        @$"SELECT t1.*, t2.name as customer_name
                   FROM {TableName} t1
                   JOIN customer t2 
                       on t1.customer_id = t2.id  
                WHERE id = @id", new { id }
    );
}