using System.Threading.Tasks;
using MiniFlexCrmApi.Api.Models;

namespace MiniFlexCrmApi.Api.Services;

public interface IUserService : IBaseService<UserModel>
{
    Task<bool> TryEnableUserAsync(int callerTenant, int userId);
    
    Task<bool> TryDisableUserAsync(int callerTenant, int userId);
}