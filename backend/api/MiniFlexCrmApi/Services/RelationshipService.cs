using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Db.Repos;
using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Services;

public class RelationshipService(IRelationshipRepo repo) : BaseService<RelationshipDbModel, RelationshipModel>(repo), IRelationshipService
{
    protected override RelationshipModel DbModelToApiModel(RelationshipDbModel model) => Converter.From(model);
    protected override RelationshipDbModel ApiModelToDbModel(RelationshipModel model) => Converter.To(model);
}