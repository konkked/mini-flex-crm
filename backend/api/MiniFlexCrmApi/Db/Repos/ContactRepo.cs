using MiniFlexCrmApi.Db.Models;

namespace MiniFlexCrmApi.Db.Repos;

public class ContactRepo(IConnectionProvider connectionProvider)
    : TenantBoundDbEntityRepo<ContactDbModel>(connectionProvider), IContactRepo;