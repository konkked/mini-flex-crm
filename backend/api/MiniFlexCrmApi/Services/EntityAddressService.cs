using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Db.Repos;
using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Services;

public class EntityAddressService(IEntityAddressRepo repo) : BaseService<EntityAddressDbModel, EntityAddressModel>(repo), IEntityAddressService
{
    protected override EntityAddressModel DbModelToApiModel(EntityAddressDbModel model) => Converter.From(model);
    protected override EntityAddressDbModel ApiModelToDbModel(EntityAddressModel model) => Converter.To(model);
}