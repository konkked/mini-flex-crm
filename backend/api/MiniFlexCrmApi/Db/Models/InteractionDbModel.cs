namespace MiniFlexCrmApi.Db.Models;

public class InteractionDbModel : TenantBoundDbEntity
{
    public string Type { get; set; }
    public DateTime InteractionDate { get; set; }
    public string? Notes { get; set; }
    public dynamic? Attributes { get; set; }
    public int? CustomerId { get; set; }
    public int? ContactId { get; set; }
    public int? LeadId { get; set; }
}