namespace MiniFlexCrmApi.Db.Models;

public class TenantBoundDbEntity : DbEntity
{
    public int TenantId { get; set; }
    
    [IgnoreForUpdate]
    public string? TenantName { get; set; }
}