using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Services;

public interface IUserService : ITenantBoundBaseService<UserModel>
{
    Task<bool> TryEnableAsync(int callerTenant, int userId);
    
    Task<bool> TryDisableAsync(int callerTenant, int userId);
}