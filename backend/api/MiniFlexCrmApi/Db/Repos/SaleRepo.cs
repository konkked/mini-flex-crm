using MiniFlexCrmApi.Db.Models;

namespace MiniFlexCrmApi.Db.Repos;

public class SaleRepo(IConnectionProvider connectionProvider)
    : TenantBoundDbEntityRepo<SaleDbModel>(connectionProvider), ISaleRepo;