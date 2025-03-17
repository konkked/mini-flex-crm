using MiniFlexCrmApi.Db.Models;

namespace MiniFlexCrmApi.Db.Repos;

public class ProductRepo(IConnectionProvider connectionProvider)
    : TenantBoundDbEntityRepo<ProductDbModel>(connectionProvider), IProductRepo;