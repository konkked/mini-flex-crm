using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Db.Repos;

public class AddressRepo(IConnectionProvider connectionProvider, RequestContext context)
    : TenantBoundDbEntityRepo<AddressDbModel>(connectionProvider, context), IAddressRepo;