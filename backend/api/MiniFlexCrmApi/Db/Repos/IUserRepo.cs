using MiniFlexCrmApi.Db.Models;

namespace MiniFlexCrmApi.Db.Repos;

public interface IUserRepo : ITenantBoundDbEntityRepo<UserDbModel>
{
    Task<UserDbModel?> FindByUsernameAsync(string username);
    Task<bool> EnableUserByUsernameAsync(string username);
    Task<bool> ExistsByUsernameAsync(string requestUsername);
    Task<bool> IsEnabledAsync(int id);
}