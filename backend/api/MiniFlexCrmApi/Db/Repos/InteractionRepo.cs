using MiniFlexCrmApi.Db.Models;

namespace MiniFlexCrmApi.Db.Repos;

public class InteractionRepo(IConnectionProvider connectionProvider)
    : TenantBoundDbEntityRepo<InteractionDbModel>(connectionProvider), IInteractionRepo;