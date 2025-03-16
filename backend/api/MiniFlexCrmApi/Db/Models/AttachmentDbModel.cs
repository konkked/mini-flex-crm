namespace MiniFlexCrmApi.Db.Models;
public class AttachmentDbModel
{
    public int Id { get; set; }
    public string Path { get; set; }
    public byte[] FileContent { get; set; }
    public string Ext { get; set; } // Computed from path
    public string Notes { get; set; } // User-provided
    public int TenantId { get; set; }
    public long CreatedTs { get; set; }
    public long UpdatedTs { get; set; }
}