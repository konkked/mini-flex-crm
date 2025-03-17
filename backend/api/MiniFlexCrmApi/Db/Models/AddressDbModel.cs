namespace MiniFlexCrmApi.Db.Models;

public class AddressDbModel : TenantBoundDbEntity
{
    public string Content { get; set; }
    public double? Lat { get; set; }
    public double? Lng { get; set; }
    public dynamic? Attributes { get; set; }
}