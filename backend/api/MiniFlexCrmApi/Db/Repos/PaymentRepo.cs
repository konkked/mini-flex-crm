using MiniFlexCrmApi.Db.Models;

namespace MiniFlexCrmApi.Db.Repos;

public class PaymentRepo(IConnectionProvider connectionProvider)
    : TenantBoundDbEntityRepo<PaymentDbModel>(connectionProvider), IPaymentRepo;