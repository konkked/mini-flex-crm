using System.Text.Json.Serialization;
using MiniFlexCrmApi.Serialization;

namespace MiniFlexCrmApi.Db.Models;

public class RelationshipDbModel: DbEntity
{
    [IgnoreForUpdate]
    public string? AccountName { get; set; }
    public int AccountId { get; set; }
    
    [JsonConverter(typeof(AttributesJsonConverter))]
    public dynamic Attributes { get; set; }
    public int EntityId { get; set; }
    public string? Entity { get; set; }
}