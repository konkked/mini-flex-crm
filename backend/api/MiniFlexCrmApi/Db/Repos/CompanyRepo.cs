using MiniFlexCrmApi.Db.Models;

namespace MiniFlexCrmApi.Db.Repos;

public class CompanyRepo(IConnectionProvider connectionProvider)
    : TenantBoundDbEntityRepo<CompanyDbModel>(connectionProvider), ICompanyRepo;