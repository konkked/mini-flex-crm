namespace MiniFlexCrmApi.Db.Models;

public class LeadDbModel : TenantBoundDbEntity
{
    public string Status { get; set; }
    public string LeadDataOrigin { get; set; }
    public string Name { get; set; }
    public string? CompanyName { get; set; }
    public string? Industry { get; set; }
    public int? ApproximateCompanySize { get; set; }
    public int? ApproximateRevenue { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public dynamic? Attributes { get; set; }
}