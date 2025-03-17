namespace MiniFlexCrmApi.Models;

public record LeadModel : TenantBoundBaseModel
{
    public LeadStatusType Status { get; set; }
    public LeadDataOriginType LeadDataOrigin { get; set; }
    public string Name { get; set; }
    public string? CompanyName { get; set; }
    public string? Industry { get; set; }
    public int? ApproximateCompanySize { get; set; }
    public int? ApproximateRevenue { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public dynamic? Attributes { get; set; }
}