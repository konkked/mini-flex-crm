using Dapper;
using MiniFlexCrmApi.Db.Models;

namespace MiniFlexCrmApi.Db.Repos;

public class AttachmentRepo(IConnectionProvider connectionProvider) : IAttachmentRepo
{
    public async Task<int> AddAsync(AttachmentDbModel attachment)
    {
        await using var connection = connectionProvider.GetConnection();
        var id = await connection.QuerySingleOrDefaultAsync<int>(@"
            INSERT INTO attachment (path, file_content, notes, tenant_id, updated_by_id)
            VALUES (@Path, @FileContent, @Notes, @TenantId, @UpdatedById)
            ON CONFLICT (path) DO NOTHING
            RETURNING id", attachment);

        return id > 0 ? id : throw new HttpRequestException("Attachment already exists", null, System.Net.HttpStatusCode.Conflict);
    }

    public async Task<AttachmentDbModel> GetAsync(string path)
    {
        await using var connection = connectionProvider.GetConnection();
        return await connection.QuerySingleOrDefaultAsync<AttachmentDbModel>( @"
            SELECT id, path, file_content, ext, notes, tenant_id, created_ts, updated_ts, updated_by_id
            FROM attachment
            WHERE path = @Path", new { Path = path });
    }

    public async Task<IEnumerable<AttachmentDbModel>> GetAllForEntity(int tenantId, string entityName, int entityId)
    {
        await using var connection = connectionProvider.GetConnection();
        return await connection.QueryAsync<AttachmentDbModel>(@"
            SELECT id, path, file_content, ext, notes, tenant_id, created_ts, updated_ts, updated_by_id
            FROM attachment
            WHERE path LIKE @PathPrefix || '%'", new
        {
            PathPrefix = $"tenant/{tenantId}/{entityName}/{entityId}/attachments/"
        });
    }
}
