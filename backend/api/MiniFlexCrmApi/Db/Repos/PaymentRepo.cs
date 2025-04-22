using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Db.Repos;

public class PaymentRepo(IConnectionProvider connectionProvider, RequestContext context)
    : TenantBoundDbEntityRepo<PaymentDbModel>(connectionProvider, context), IPaymentRepo;