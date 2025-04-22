using Dapper;
using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Db.Repos;

public class UserRepo(IConnectionProvider connectionProvider, RequestContext context) 
    : TenantBoundDbEntityRepo<UserDbModel>(connectionProvider, context), IUserRepo
{
    public async Task<UserDbModel?> FindByUsernameAsync(string username)
    {
        await using var connection = ConnectionProvider.GetConnection();
        await connection.OpenAsync().ConfigureAwait(false);
        return await connection.QueryFirstOrDefaultAsync<UserDbModel>(
            $@"SELECT * FROM {TableName} where username = @username", new { username })
            .ConfigureAwait(false);
    }

    public async Task<bool> EnableUserByUsernameAsync(string username)
    {
        await using var connection = ConnectionProvider.GetConnection();
        await connection.OpenAsync().ConfigureAwait(false);
        return await connection.ExecuteAsync(
                $@"UPDATE {TableName} SET enabled=true WHERE username = @username", new { username })
            .ConfigureAwait(false) == 1;
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