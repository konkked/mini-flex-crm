using Dapper;
using MiniFlexCrmApi.Db;
using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Services;

public class NotesService(IConnectionProvider connectionProvider) : INotesService
{
    public async Task<IEnumerable<NoteModel>> ListAsync(int userId, int tenantId, string route)
    {
        await using var connection = connectionProvider.GetConnection();
        var notes = await connection.QueryAsync<NoteModel>(
            "SELECT * FROM notes WHERE user_id = @UserId AND tenant_id = @TenantId AND route = @Route",
            new { UserId = userId, TenantId = tenantId, Route = route });
        return notes;
    }

    public async Task<int> CreateAsync(NoteModel note)
    {
        await using var connection = connectionProvider.GetConnection();
        return await connection.QuerySingleAsync<int>(
            @"INSERT INTO notes (user_id, tenant_id, route, title, content, pinned) 
                  VALUES (@UserId, @TenantId, @Route, @Title, @Content, @Pinned) RETURNING id",
            note);
    }

    public async Task DeleteAsync(int id, int userId)
    {
        await using var connection = connectionProvider.GetConnection();
        await connection.ExecuteAsync(
            "DELETE FROM notes WHERE id = @Id AND user_id = @UserId",
            new { Id = id, UserId = userId });
    }

    public async Task TogglePinAsync(int id, int userId)
    {
        await using var connection = connectionProvider.GetConnection();
        await connection.ExecuteAsync(
            "UPDATE notes SET pinned = NOT pinned WHERE id = @Id AND user_id = @UserId",
            new { Id = id, UserId = userId });
    }
}