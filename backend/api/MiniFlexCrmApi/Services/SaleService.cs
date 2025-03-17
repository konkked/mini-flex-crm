using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Db.Repos;
using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Services;

public class SaleService(ISaleRepo repo) : TenantBoundBaseService<SaleDbModel, SaleModel>(repo), ISaleService
{
    protected override SaleModel DbModelToApiModel(SaleDbModel model) => Converter.From(model);
    protected override SaleDbModel ApiModelToDbModel(SaleModel model) => Converter.To(model);
}