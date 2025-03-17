using MiniFlexCrmApi.Db.Models;

namespace MiniFlexCrmApi.Db.Repos;

public class EntityAddressRepo(IConnectionProvider connectionProvider)
    : DbEntityRepo<EntityAddressDbModel>(connectionProvider), IEntityAddressRepo;