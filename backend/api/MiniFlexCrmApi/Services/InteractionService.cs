using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Db.Repos;
using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Services;

public class InteractionService(IInteractionRepo repo) : TenantBoundBaseService<InteractionDbModel, InteractionModel>(repo), IInteractionService
{
    protected override InteractionModel DbModelToApiModel(InteractionDbModel model) => Converter.From(model);
    protected override InteractionDbModel ApiModelToDbModel(InteractionModel model) => Converter.To(model);
}