namespace MiniFlexCrmApi.Db.Models;

public class ProductDbModel : TenantBoundDbEntity
{
    public string Name { get; set; }
    public decimal SuggestedPrice { get; set; }
    public int? TermMonths { get; set; }
    public dynamic? Attributes { get; set; }
}