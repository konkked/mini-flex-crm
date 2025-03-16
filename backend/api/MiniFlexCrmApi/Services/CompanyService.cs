using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Db.Repos;
using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Services;

public class CompanyService(ICompanyRepo repo) : TenantBoundBaseService<CompanyDbModel, CompanyModel>(repo), ICompanyService
{
    protected override CompanyModel DbModelToApiModel(CompanyDbModel model) => Converter.From(model);
    protected override CompanyDbModel ApiModelToDbModel(CompanyModel model) => Converter.To(model);
}