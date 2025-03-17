namespace MiniFlexCrmApi.Models;

public record EntityAddressModel : BaseApiModel
{
    public int SignificanceOrdinal { get; set; }
    public string EntityName { get; set; }
    public int EntityId { get; set; }
    public int AddressId { get; set; }
}