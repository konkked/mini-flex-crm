using MiniFlexCrmApi.Api.Models;
using MiniFlexCrmApi.Models.Public;

namespace MiniFlexCrmApi.Auth;

public interface IUserService : IBaseEntityService<UserModel>
{
    
    Task<bool> TryEnableUser(RequestContext context, int userId);
    
    Task<bool> TryDisableUser(RequestContext context, int userId);
}