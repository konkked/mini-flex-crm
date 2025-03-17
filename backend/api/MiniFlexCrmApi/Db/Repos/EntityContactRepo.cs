using MiniFlexCrmApi.Db.Models;

namespace MiniFlexCrmApi.Db.Repos;

public class EntityContactRepo(IConnectionProvider connectionProvider)
    : DbEntityRepo<EntityContactDbModel>(connectionProvider), IEntityContactRepo;