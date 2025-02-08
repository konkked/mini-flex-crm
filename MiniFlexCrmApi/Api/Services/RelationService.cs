using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Db.Repos;
using MiniFlexCrmApi.Models.Public;

namespace MiniFlexCrmApi.Api.Services;

public class RelationService(IRelationRepo repo) : BaseService<RelationDbModel, RelationModel>(repo), IRelationService
{
    protected override RelationModel ConvertToApiModel(RelationDbModel model) => Converter.From(model);
    protected override RelationDbModel ConvertToDbModel(RelationModel model) => Converter.To(model);
}