using MiniFlexCrmApi.Api.Models;

namespace MiniFlexCrmApi.Api.Auth;

public interface IUserService : IBaseEntityService<UserModel>
{
    
    Task<bool> TryEnableUser(RequestContext context, int userId);
    
    Task<bool> TryDisableUser(RequestContext context, int userId);
}