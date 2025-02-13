using MiniFlexCrmApi.Db.Models;

namespace MiniFlexCrmApi.Db.Repos;

public class CustomerRepo(IConnectionProvider connectionProvider)
    : TenantBoundDbEntityRepo<CustomerDbModel>(connectionProvider), ICustomerRepo;