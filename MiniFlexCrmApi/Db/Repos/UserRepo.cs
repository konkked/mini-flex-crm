using Dapper;
using MiniFlexCrmApi.Db.Models;

namespace MiniFlexCrmApi.Db.Repos;

public class UserRepo(IConnectionProvider connectionProvider) : DbEntityRepo<UserDbModel>(connectionProvider), IUserRepo
{
    public Task<UserDbModel?> FindByUsernameAsync(string username) 
        => ConnectionProvider.Connection.QueryFirstOrDefaultAsync<UserDbModel>(
            $@"SELECT * FROM {TableName} where username=@username", new { username });

    public Task<bool> ExistsByUsernameAsync(string username) => ConnectionProvider.Connection.ExecuteScalarAsync<bool>(
        $"SELECT EXISTS(SELECT 1 FROM {TableName} WHERE username = @username)", new { username }
    );
}