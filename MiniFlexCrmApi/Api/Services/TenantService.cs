using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Db.Repos;
using MiniFlexCrmApi.Models.Public;

namespace MiniFlexCrmApi.Api.Services;

public class TenantService(ITenantRepo repo) : BaseService<TenantDbModel, TenantModel>(repo), ITenantService
{
    protected override TenantModel ConvertToApiModel(TenantDbModel model) => Converter.From(model);
    protected override TenantDbModel ConvertToDbModel(TenantModel model) => Converter.To(model);
}