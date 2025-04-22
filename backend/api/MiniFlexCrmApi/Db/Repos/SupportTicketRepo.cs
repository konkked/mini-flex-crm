using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Db.Repos;

public class SupportTicketRepo(IConnectionProvider connectionProvider, RequestContext context)
    : TenantBoundDbEntityRepo<SupportTicketDbModel>(connectionProvider, context), ISupportTicketRepo;