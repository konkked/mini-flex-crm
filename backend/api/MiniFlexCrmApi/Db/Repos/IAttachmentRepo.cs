using MiniFlexCrmApi.Db.Models;

namespace MiniFlexCrmApi.Db.Repos;

public interface IAttachmentRepo
{
    Task<int> AddAsync(AttachmentDbModel attachment);
    Task<AttachmentDbModel> GetAsync(string path);
    Task<IEnumerable<AttachmentDbModel>> GetAllForEntity(int tenantId, string entityName, int entityId);
}