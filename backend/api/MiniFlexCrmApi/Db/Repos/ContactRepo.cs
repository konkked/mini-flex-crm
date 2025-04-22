using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Db.Repos;

public class ContactRepo(IConnectionProvider connectionProvider, RequestContext context)
    : TenantBoundDbEntityRepo<ContactDbModel>(connectionProvider, context), IContactRepo;