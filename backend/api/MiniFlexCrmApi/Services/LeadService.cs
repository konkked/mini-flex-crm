using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Db.Repos;
using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Services;

public class LeadService(ILeadRepo repo) : TenantBoundBaseService<LeadDbModel, LeadModel>(repo), ILeadService
{
    protected override LeadModel DbModelToApiModel(LeadDbModel model) => Converter.From(model);
    protected override LeadDbModel ApiModelToDbModel(LeadModel model) => Converter.To(model);
}