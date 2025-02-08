namespace MiniFlexCrmApi.Db.Models;

public class RelationDbModel: DbEntity
{
    [IgnoreForUpdate]
    public string? CustomerName { get; set; }
    public int CustomerId { get; set; }
    public int EntityId { get; set; }
    public string? Entity { get; set; }
}