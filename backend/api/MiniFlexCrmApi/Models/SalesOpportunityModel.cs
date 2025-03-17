namespace MiniFlexCrmApi.Models;

public record SalesOpportunityModel : TenantBoundBaseModel
{
    public string Title { get; set; }
    public string Status { get; set; }
    public decimal Value { get; set; }
    public dynamic? Attributes { get; set; }
    public int? CustomerId { get; set; }
    public int? LeadId { get; set; }
}