namespace MiniFlexCrmApi.Db.Models;

public class TenantDbModel: DbEntity
{
    public string Name { get; set; }
    public string ShortId { get; set; }
    public dynamic Attributes { get; set; }
}