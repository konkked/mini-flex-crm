using System.Collections.Generic;
using System.Threading.Tasks;
using MiniFlexCrmApi.Db.Models;

namespace MiniFlexCrmApi.Db.Repos;

public interface IRelationRepo : IRepo<RelationDbModel>
{
    Task<Dictionary<string, dynamic[]>> GetCustomerRelationshipsAsync(int customerId);
}