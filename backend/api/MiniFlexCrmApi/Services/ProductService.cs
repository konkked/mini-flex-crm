using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Db.Repos;
using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Services;

public class ProductService(IProductRepo repo) : TenantBoundBaseService<ProductDbModel, ProductModel>(repo), IProductService
{
    protected override ProductModel DbModelToApiModel(ProductDbModel model) => Converter.From(model);
    protected override ProductDbModel ApiModelToDbModel(ProductModel model) => Converter.To(model);
}