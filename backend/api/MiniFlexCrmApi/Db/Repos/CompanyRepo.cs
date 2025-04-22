using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Db.Repos;

public class CompanyRepo(IConnectionProvider connectionProvider, RequestContext context)
    : TenantBoundDbEntityRepo<CompanyDbModel>(connectionProvider, context), ICompanyRepo;