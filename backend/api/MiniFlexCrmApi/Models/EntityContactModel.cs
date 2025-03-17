namespace MiniFlexCrmApi.Models;

public record EntityContactModel : TenantBoundBaseModel
{
    public int SignificanceOrdinal { get; set; }
    public string EntityName { get; set; }
    public int EntityId { get; set; }
    public int ContactId { get; set; }
}