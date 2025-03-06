using MiniFlexCrmApi.Db.Models;

namespace MiniFlexCrmApi.Db.Repos;

public interface ICustomerRepo : ITenantBoundDbEntityRepo<CustomerDbModel>;