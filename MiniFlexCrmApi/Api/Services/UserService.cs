using MiniFlexCrmApi.Api.Models;
using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Db.Repos;

namespace MiniFlexCrmApi.Api.Services;

public class UserService(IUserRepo repo) : BaseService<UserDbModel, UserModel>(repo), IUserService
{
    protected override UserModel ConvertToApiModel(UserDbModel model) => Converter.From(model);
    protected override UserDbModel ConvertToDbModel(UserModel model) => Converter.To(model);
}