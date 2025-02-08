using MiniFlexCrmApi.Api.Models;
using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Db.Repos;

namespace MiniFlexCrmApi.Api.Services;

public class CompanyService(ICompanyRepo repo) : BaseService<CompanyDbModel, CompanyModel>(repo), ICompanyService
{
    protected override CompanyModel ConvertToApiModel(CompanyDbModel model) => Converter.From(model);
    protected override CompanyDbModel ConvertToDbModel(CompanyModel model) => Converter.To(model);
}