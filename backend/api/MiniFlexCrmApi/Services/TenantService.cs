using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Db.Repos;
using MiniFlexCrmApi.Models;
using MiniFlexCrmApi.Serialization;
using MiniFlexCrmApi.Util;

namespace MiniFlexCrmApi.Services;

public class TenantService(ITenantRepo repo) : BaseService<TenantDbModel, TenantModel>(repo), ITenantService
{
    protected override TenantModel DbModelToApiModel(TenantDbModel model) 
        => Converter.From(model);
    
    protected override TenantDbModel ApiModelToDbModel(TenantModel model) 
        => Converter.To(model);
}