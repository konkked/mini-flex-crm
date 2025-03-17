namespace MiniFlexCrmApi.Models;

public record SaleModel : TenantBoundBaseModel 
{
    public string Title { get; set; }
    public string? Description { get; set; }
    public decimal Value { get; set; }
    public int? TermMonths { get; set; }
    public dynamic? Attributes { get; set; }
    public int? SalesOpportunityId { get; set; }
}