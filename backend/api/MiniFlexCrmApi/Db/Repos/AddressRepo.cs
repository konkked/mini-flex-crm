using MiniFlexCrmApi.Db.Models;

namespace MiniFlexCrmApi.Db.Repos;

public class AddressRepo(IConnectionProvider connectionProvider)
    : TenantBoundDbEntityRepo<AddressDbModel>(connectionProvider), IAddressRepo;