using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Db.Repos;

public class ProductRepo(IConnectionProvider connectionProvider, RequestContext context)
    : TenantBoundDbEntityRepo<ProductDbModel>(connectionProvider, context), IProductRepo;