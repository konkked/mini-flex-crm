using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Db.Repos;

public class SaleRepo(IConnectionProvider connectionProvider, RequestContext context)
    : TenantBoundDbEntityRepo<SaleDbModel>(connectionProvider, context), ISaleRepo;