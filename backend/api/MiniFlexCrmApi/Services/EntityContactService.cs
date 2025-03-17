using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Db.Repos;
using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Services;

public class EntityContactService(IEntityContactRepo repo) : BaseService<EntityContactDbModel, EntityContactModel>(repo), IEntityContactService
{
    protected override EntityContactModel DbModelToApiModel(EntityContactDbModel model) => Converter.From(model);
    protected override EntityContactDbModel ApiModelToDbModel(EntityContactModel model) => Converter.To(model);
}