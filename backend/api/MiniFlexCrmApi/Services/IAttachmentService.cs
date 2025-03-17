using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Services;

public interface IAttachmentService
{
    Task<int> UploadAsync(IFormFile file, string name, string ext, int tenantId, 
        string entityName, int entityId, string notes = null);

    Task<Attachment> GetAsync(string path);
    Task<IEnumerable<Attachment>> GetAllForEntityAsync(int tenantId, string entityName, int entityId);
}