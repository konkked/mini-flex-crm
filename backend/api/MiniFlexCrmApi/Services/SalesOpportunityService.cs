using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Db.Repos;
using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Services;

public class SalesOpportunityService(ISalesOpportunityRepo repo) : TenantBoundBaseService<SalesOpportunityDbModel, SalesOpportunityModel>(repo), ISalesOpportunityService
{
    protected override SalesOpportunityModel DbModelToApiModel(SalesOpportunityDbModel model) => Converter.From(model);
    protected override SalesOpportunityDbModel ApiModelToDbModel(SalesOpportunityModel model) => Converter.To(model);
}