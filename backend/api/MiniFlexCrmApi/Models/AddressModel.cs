namespace MiniFlexCrmApi.Models;

public record AddressModel : LinkedTenantBoundBaseModel
{
    public string Content { get; set; }
    public double? Lat { get; set; }
    public double? Lng { get; set; }
    public dynamic? Attributes { get; set; }
}