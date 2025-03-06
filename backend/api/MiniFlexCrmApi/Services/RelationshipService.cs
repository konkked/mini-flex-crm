using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Db.Repos;
using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Services;

public class RelationshipService(IRelationshipRepo repo) : BaseService<RelationshipDbModel, RelationshipModel>(repo), IRelationService
{
    protected override RelationshipModel ConvertToApiModel(RelationshipDbModel model) => Converter.From(model);
    protected override RelationshipDbModel ConvertToDbModel(RelationshipModel model) => Converter.To(model);
}