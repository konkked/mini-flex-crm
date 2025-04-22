using System.Text.Json;
using Dapper;
using MiniFlexCrmApi.Db.Models;

namespace MiniFlexCrmApi.Db.Repos;

public class RelationshipRepo : DbEntityRepo<RelationshipDbModel>, IRelationshipRepo
{
    public RelationshipRepo(IConnectionProvider connectionProvider) : base(connectionProvider)
    {
    }

    /// <summary>
    /// Retrieves aggregated relationships for a given account.
    /// Calls PostgreSQL function get_account_entities(account_id) and returns parsed data.
    /// </summary>
    public async Task<Dictionary<string,dynamic[]>> GetAccountRelationshipsAsync(int accountId)
    {
        await using var connection = ConnectionProvider.GetConnection();
        await connection.OpenAsync().ConfigureAwait(false);
        var result = await connection
            .ExecuteScalarAsync<string>(
                "SELECT get_account_relationships(@accountId);", 
                new { accountId })
            .ConfigureAwait(false);

        // Convert JSON result into a Dictionary<string, dynamic[]>
        
        return JsonSerializer.Deserialize<Dictionary<string, dynamic[]>>(result);
    }
    
    public override IAsyncEnumerable<RelationshipDbModel> GetSomeAsync(int limit) => GetSomeAsync(limit, 0);

    public override async IAsyncEnumerable<RelationshipDbModel> GetSomeAsync(int limit, int offset)
    {
        await using var connection = ConnectionProvider.GetConnection();
        await connection.OpenAsync().ConfigureAwait(false);
        var results = await connection.QueryAsync<RelationshipDbModel>(
            @$"SELECT t1.*, t2.name as account_name
                   FROM {TableName} t1
                   JOIN account t2 
                       on t1.account_id = t2.id   
                   ORDER BY t1.id ASC 
                   LIMIT @count
                   OFFSET @offset",
            new { offset, count = limit }
        ).ConfigureAwait(false);

        foreach (var item in results)
            yield return item;
    }

    public override async Task<RelationshipDbModel?> FindAsync(int id)
    {
        await using var connection = ConnectionProvider.GetConnection();
        await connection.OpenAsync().ConfigureAwait(false);
        return await connection.QueryFirstOrDefaultAsync<RelationshipDbModel>(
            @$"SELECT t1.*, t2.name as account_name
                   FROM {TableName} t1
                   JOIN account t2 
                       on t1.account_id = t2.id  
                WHERE id = @id", new { id }
        ).ConfigureAwait(false);
    }
}