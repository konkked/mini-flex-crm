using MiniFlexCrmApi.Db.Models;

namespace MiniFlexCrmApi.Db.Repos;

public class SupportTicketRepo(IConnectionProvider connectionProvider)
    : TenantBoundDbEntityRepo<SupportTicketDbModel>(connectionProvider), ISupportTicketRepo;