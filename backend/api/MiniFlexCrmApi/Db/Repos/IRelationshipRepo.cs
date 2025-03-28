using MiniFlexCrmApi.Db.Models;

namespace MiniFlexCrmApi.Db.Repos;

public interface IRelationshipRepo : IRepo<RelationshipDbModel>
{
    Task<Dictionary<string, dynamic[]>> GetCustomerRelationshipsAsync(int customerId);
}