using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Db.Repos;
using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Services;

public class PaymentService(IPaymentRepo repo) : TenantBoundBaseService<PaymentDbModel, PaymentModel>(repo), IPaymentService
{
    protected override PaymentModel DbModelToApiModel(PaymentDbModel model) => Converter.From(model);
    protected override PaymentDbModel ApiModelToDbModel(PaymentModel model) => Converter.To(model);
}