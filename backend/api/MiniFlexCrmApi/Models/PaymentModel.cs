namespace MiniFlexCrmApi.Models;

public record PaymentModel : TenantBoundBaseModel
{
    public PaymentType Type { get; set; }
    public string Title { get; set; }
    public decimal Value { get; set; }
    public int SaleId { get; set; }
    public dynamic? Attributes { get; set; }
}