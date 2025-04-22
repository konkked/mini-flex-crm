using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Db.Repos;

public class SalesOpportunityRepo(IConnectionProvider connectionProvider, RequestContext context)
    : TenantBoundDbEntityRepo<SalesOpportunityDbModel>(connectionProvider, context), ISalesOpportunityRepo;