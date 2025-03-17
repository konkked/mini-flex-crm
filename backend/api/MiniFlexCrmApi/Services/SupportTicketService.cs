using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Db.Repos;
using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Services;

public class SupportTicketService(ISupportTicketRepo repo) : TenantBoundBaseService<SupportTicketDbModel, SupportTicketModel>(repo), ISupportTicketService
{
    protected override SupportTicketModel DbModelToApiModel(SupportTicketDbModel model) => Converter.From(model);
    protected override SupportTicketDbModel ApiModelToDbModel(SupportTicketModel model) => Converter.To(model);
}