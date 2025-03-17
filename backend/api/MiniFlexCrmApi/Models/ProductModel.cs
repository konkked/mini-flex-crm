namespace MiniFlexCrmApi.Models;

public record ProductModel : TenantBoundBaseModel
{
    public string Name { get; set; }
    public decimal SuggestedPrice { get; set; }
    public int? TermMonths { get; set; }
    public dynamic? Attributes { get; set; }
}