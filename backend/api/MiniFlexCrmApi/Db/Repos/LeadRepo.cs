using MiniFlexCrmApi.Db.Models;

namespace MiniFlexCrmApi.Db.Repos;

public class LeadRepo(IConnectionProvider connectionProvider)
    : TenantBoundDbEntityRepo<LeadDbModel>(connectionProvider), ILeadRepo;