using System.Text.Json.Serialization;
using MiniFlexCrmApi.Serialization;

namespace MiniFlexCrmApi.Db.Models;

public class TenantDbModel: DbEntity
{
    public string Name { get; set; }
    public string ShortId { get; set; }
    
    [JsonConverter(typeof(AttributesJsonConverter))]
    public dynamic? Attributes { get; set; }
    public string? Theme { get; set; }
}