namespace MiniFlexCrmApi.Db.Models;

public class LinkingDbModel : DbEntity
{
    [IgnoreForUpdate]
    public int SignificanceOrdinal { get; set; }
    public string EntityName { get; set; }
    public int EntityId { get; set; }
}

public class EntityContactDbModel : LinkingDbModel
{
    public int ContactId { get; set; }
}