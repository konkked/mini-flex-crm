using MiniFlexCrmApi.Db.Models;

namespace MiniFlexCrmApi.Db.Repos;

public class SalesOpportunityRepo(IConnectionProvider connectionProvider)
    : TenantBoundDbEntityRepo<SalesOpportunityDbModel>(connectionProvider), ISalesOpportunityRepo;