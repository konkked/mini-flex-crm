using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Db.Models;

public class PaymentDbModel : TenantBoundDbEntity
{
    public string Type { get; set; }
    public string Title { get; set; }
    public decimal Value { get; set; }
    public int SaleId { get; set; }
    public dynamic? Attributes { get; set; }
}