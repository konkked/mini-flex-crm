using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Db.Repos;

public class LeadRepo(IConnectionProvider connectionProvider, RequestContext context)
    : TenantBoundDbEntityRepo<LeadDbModel>(connectionProvider, context), ILeadRepo;