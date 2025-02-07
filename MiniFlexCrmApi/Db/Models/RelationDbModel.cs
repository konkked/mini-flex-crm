namespace MiniFlexCrmApi.Db.Models;

public class RelationDbModel: DbEntity
{
    public int CustomerId { get; set; }
    public int EntityId { get; set; }
    public string Entity { get; set; }
}