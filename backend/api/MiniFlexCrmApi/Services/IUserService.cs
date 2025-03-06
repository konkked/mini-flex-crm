using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Services;

public interface IUserService : ITenantBoundBaseService<UserModel>
{
    Task<bool> TryEnableUserAsync(int callerTenant, int userId);
    
    Task<bool> TryDisableUserAsync(int callerTenant, int userId);
}