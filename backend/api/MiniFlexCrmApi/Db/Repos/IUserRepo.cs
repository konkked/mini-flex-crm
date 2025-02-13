using MiniFlexCrmApi.Db.Models;

namespace MiniFlexCrmApi.Db.Repos;

public interface IUserRepo : IRepo<UserDbModel>
{
    Task<UserDbModel?> FindByUsernameAsync(string username);
    Task<bool> ExistsByUsernameAsync(string requestUsername);
    Task<bool> IsEnabledAsync(int id);
}