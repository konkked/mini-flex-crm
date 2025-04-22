namespace MiniFlexCrmApi.Db.Models;

public class InteractionDbModel : TenantBoundDbEntity
{
    public int Id { get; set; }
    public string Type { get; set; } 
    public DateTime InteractionDate { get; set; }
    public string? Notes { get; set; }
    public dynamic? Attributes { get; set; }
    public int? AccountId { get; set; }
    public int? ContactId { get; set; }
    public int? LeadId { get; set; }
    public int? DealId { get; set; }
}