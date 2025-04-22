using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Db.Repos;

public class InteractionRepo(IConnectionProvider connectionProvider, RequestContext context)
    : TenantBoundDbEntityRepo<InteractionDbModel>(connectionProvider, context), IInteractionRepo;