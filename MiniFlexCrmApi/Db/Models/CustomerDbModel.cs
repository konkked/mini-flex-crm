using System.Text.Json.Serialization;

namespace MiniFlexCrmApi.Db.Models;

public class CustomerDbModel: DbEntity
{
    public string Name { get; set; }
    
    // Map JSON attributes as a dictionary or a specific type
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public dynamic Attributes { get; set; }

    public int TenantId { get; set; }
}