using MiniFlexCrmApi.Db;
using MiniFlexCrmApi.Models;
using System.Threading.Tasks;
using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Db.Repos;

namespace MiniFlexCrmApi.Services
{
    public interface IAttachmentService
    {
        Task<int> UploadAsync(IFormFile file, string name, string ext, int tenantId, 
            string entityName, int entityId, string notes = null);

        Task<Attachment> GetAsync(string path);
        Task<IEnumerable<Attachment>> GetAllForEntityAsync(int tenantId, string entityName, int entityId);
    }

    public class AttachmentService(IAttachmentRepo repo) : IAttachmentService
    {
        public async Task<int> UploadAsync(IFormFile file, string name, string ext, int tenantId, 
            string entityName, int entityId, string notes = null)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("No file uploaded.");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("File name is required.");
            if (string.IsNullOrEmpty(ext))
                throw new ArgumentException("File extension is required.");

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var fileContent = memoryStream.ToArray();

            var path = $"tenant/{tenantId}/{entityName}/{entityId}/attachments/{name}.{ext}";
            var attachment = new AttachmentDbModel
            {
                Path = path,
                FileContent = fileContent,
                Notes = notes,
                TenantId = tenantId
            };

            var id = await repo.AddAsync(attachment);
            return id;
        }

        public async Task<Attachment> GetAsync(string path)
        {
            var attachment = await repo.GetAsync(path);
            if (attachment == null)
                throw new KeyNotFoundException("Attachment not found.");
            return Converter.From(attachment);
        }

        public async Task<IEnumerable<Attachment>> GetAllForEntityAsync(int tenantId, string entityName, int entityId)
        {
            var results = await repo.GetAllForEntity(tenantId, entityName, entityId);
            return results.Select(Converter.From);
        }
    }
}