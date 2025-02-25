using System.Threading.Tasks;
using Dapper;
using MiniFlexCrmApi.Db.Models;

namespace MiniFlexCrmApi.Db.Repos;

public class UserRepo(IConnectionProvider connectionProvider) : TenantBoundDbEntityRepo<UserDbModel>(connectionProvider), IUserRepo
{
    public async Task<UserDbModel?> FindByUsernameAsync(string username)
    {
        await using var connection = ConnectionProvider.GetConnection();
        await connection.OpenAsync().ConfigureAwait(false);
        return await connection.QueryFirstOrDefaultAsync<UserDbModel>(
            $@"SELECT * FROM {TableName} where username = @username", new { username })
            .ConfigureAwait(false);
    }

    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        await using var connection = ConnectionProvider.GetConnection();
        await connection.OpenAsync().ConfigureAwait(false);
        return await connection.ExecuteScalarAsync<bool>(
            $"SELECT EXISTS(SELECT 1 FROM {TableName} WHERE username = @username)", new { username }
        );
    }

    public async Task<bool> IsEnabledAsync(int id)
    {
        await using var connection = ConnectionProvider.GetConnection();
        await connection.OpenAsync().ConfigureAwait(false);
        return await connection.ExecuteScalarAsync<bool>(
            $"SELECT EXISTS(SELECT 1 FROM {TableName} WHERE id = @id AND enabled = TRUE)", new { id }
        );
    }
}